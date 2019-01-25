using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XMLSchemaDefinition;

namespace ColladaSharp.Definition
{
    public partial class Collada
    {
        public abstract class BaseColladaElement<TParent> : BaseElement<TParent> where TParent : class, IElement
        {
            public COLLADA Root => RootElement as COLLADA;
            public virtual List<IID> GetIDEntries(string id) => Root.IDEntries[id];
            protected override void OnAttributesRead()
            {
                if (this is IID IDEntry && !string.IsNullOrEmpty(IDEntry.ID))
                {
                    if (!Root.IDEntries.ContainsKey(IDEntry.ID))
                        Root.IDEntries.Add(IDEntry.ID, new List<IID>() { IDEntry });
                    else
                        Root.IDEntries[IDEntry.ID].Add(IDEntry);
                }

                if (this is ISID SIDEntry && !string.IsNullOrEmpty(SIDEntry.SID))
                {
                    IElement p = SIDEntry.Parent;
                    while (true)
                    {
                        if (p is ISIDAncestor ancestor)
                        {
                            ancestor.SIDElementChildren.Add(SIDEntry);
                            break;
                        }
                        else if (p.Parent != null)
                            p = p.Parent;
                        else
                            break;
                    }
                }
            }

        }
        /// <summary>
        /// This is a url that references the unique id of another element.
        /// Can be internal or external.
        /// Internal example: url="#whateverId"
        /// External example: url="file:///some_place/doc.dae#complex_building"
        /// </summary>
        public class ColladaURI : IParsable
        {
            public string URI { get; set; }
            bool IsLocal => URI.StartsWith("#");

            public void ReadFromString(string str) => URI = str;
            public string WriteToString() => URI;

            public T GetElement<T>(IRootElement root) where T : IID
                => GetElement<T>(root as COLLADA);
            public T GetElement<T>(COLLADA root) where T : IID
            {
                List<T> elements = GetElements<T>(root);
                if (elements.Count == 0)
                    return default;
                if (elements.Count == 1)
                    return elements[0];
                throw new Exception($"{elements.Count} ID entries at '{TargetID}' of type {typeof(T).GetFriendlyName()}. Cannot determine which is which.");
            }

            public List<T> GetElements<T>(IRootElement root) where T : IID
                => GetElements<T>(root as COLLADA);
            public List<T> GetElements<T>(COLLADA root) where T : IID
                => GetElements(root).Where(x => typeof(T).IsAssignableFrom(x.GetType())).Select(x => (T)x).ToList();

            public IID GetElement(IRootElement root)
                => GetElement(root as COLLADA);
            public IID GetElement(COLLADA root)
            {
                List<IID> elements = GetElements(root);
                if (elements.Count == 0)
                    return default;
                if (elements.Count == 1)
                    return elements[0];
                throw new Exception($"{elements.Count} ID entries at '{TargetID}'. Cannot determine which is which.");
            }

            public List<IID> GetElements(IRootElement root)
                => GetElements(root as COLLADA);
            public List<IID> GetElements(COLLADA root)
                => IsLocal ? root?.GetIDEntries(URI.Substring(1)) : null;

            public string TargetID => IsLocal ? URI.Substring(1) : URI;
        }
        public class SidRef : IParsable
        {
            public string Path { get; set; }
            public void ReadFromString(string str) => Path = str;
            public string WriteToString() => Path;
            public T GetElement<T>(COLLADA root, out string selector) where T : ISID
                => (T)GetElement(root, out selector);
            public ISID GetElement(COLLADA root, out string selector)
            {
                selector = null;
                string[] parts = Path.Split('/');
                string idName = parts[0];
                List<IID> ids = root.GetIDEntries(idName);
                if (ids.Count == 0)
                    return null;
                if (ids.Count > 1)
                {
                    //Minor edge case which doesn't typically happen.
                    //TODO: determine which is which
                    throw new InvalidOperationException($"{ids.Count} ID entries named '{idName}'. Cannot determine which is which.");
                }
                ISIDAncestor ancestor = ids[0];
                for (int i = 1; i < parts.Length; ++i)
                {
                    string part = parts[i];
                    int selectorIndex = part.IndexOf('.');
                    if (selectorIndex >= 0)
                    {
                        selector = part.Substring(selectorIndex + 1);
                        part = part.Substring(0, selectorIndex);
                    }
                    else
                    {
                        int dimSelector = part.IndexOf('(');
                        if (dimSelector >= 0)
                        {
                            selector = part.Substring(dimSelector);
                            part = part.Substring(0, dimSelector);
                        }
                    }
                    ancestor = ancestor.SIDElementChildren.FirstOrDefault(x => string.Equals(x.SID, part, StringComparison.InvariantCulture));
                }
                return ancestor as ISID;
            }
        }
        [ElementName("extra")]
        [Child(typeof(Asset), 0, 1)]
        [Child(typeof(Technique), 1, -1)]
        public class Extra : BaseColladaElement<IExtra>, IID, IElementName, ITechnique, IAsset
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Extra;

            [Attr("id", false)]
            public string ID { get; set; } = null;
            [Attr("Name", false)]
            public string Name { get; set; } = null;

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

            public Asset AssetElement => GetChild<Asset>();
        }
        [ElementName("annotate")]
        public class Annotate : BaseColladaElement<IAnnotate>
        {

        }
        [ElementName("newparam")]
        [Child(typeof(Annotate), 0, -1)]
        [Child(typeof(Semantic), 0, 1)]
        [Child(typeof(Modifier), 0, 1)]
        [MultiChild(EMultiChildType.OneOfOne, typeof(Sampler2D))]
        public class NewParam : BaseColladaElement<INewParam>, ISID, IAnnotate
        {
            [Attr("sid", true)]
            public string SID { get; set; }

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

            [ElementName("semantic")]
            public class Semantic : BaseStringElement<NewParam, ElementString>
            {
                public string Value
                {
                    get => StringContent.Value;
                    set => StringContent.Value = value;
                }
            }
            [ElementName("modifier")]
            public class Modifier : BaseStringElement<NewParam, StringPrimitive<ELinkageModifier>>
            {
                public ELinkageModifier LinkageModifier
                {
                    get => StringContent.Value;
                    set => StringContent.Value = value;
                }
            }
            [ElementName("sampler2D")]
            public class Sampler2D : BaseColladaElement<NewParam>
            {

            }
            public enum ELinkageModifier
            {
                CONST,
                UNIFORM,
                VARYING,
                STATIC,
                VOLATILE,
                EXTERN,
                SHARED,
            }
        }
        [ElementName("setparam")]
        public class SetParam : BaseColladaElement<ISetParam>
        {
            [Attr("ref", true)]
            public string Reference { get; set; }
            //public IID GetElement() => GetIDEntry(ReferenceID);
        }
        [ElementName("param")]
        public class RefParam : BaseColladaElement<IRefParam>
        {
            [Attr("ref", false)]
            public string Reference { get; set; } = null;
        }
        [ElementName("param")]
        public class DataFlowParam : BaseColladaElement<IDataFlowParam>, ISID, IElementName
        {
            [Attr("sid", false)]
            public string SID { get; set; } = null;
            [Attr("Name", false)]
            public string Name { get; set; } = null;
            /// <summary>
            /// The type of the value data. This text string must be understood by the application.
            /// </summary>
            [Attr("type", true)]
            public string Type { get; set; } = null;
            /// <summary>
            /// The user-defined meaning of the parameter.
            /// </summary>
            [Attr("semantic", false)]
            public string Semantic { get; set; } = null;

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();
        }

        #region Asset
        [ElementName("asset")]
        [Child(typeof(Contributor), 0, -1)]
        [Child(typeof(Coverage), 0, 1)]
        [Child(typeof(Created), 1)]
        [Child(typeof(Keywords), 0, 1)]
        [Child(typeof(Modified), 1)]
        [Child(typeof(Revision), 0, 1)]
        [Child(typeof(Subject), 0, 1)]
        [Child(typeof(Title), 0, 1)]
        [Child(typeof(Unit), 0, 1)]
        [Child(typeof(UpAxis), 0, 1)]
        [Child(typeof(Extra), 0, -1)]
        public class Asset : BaseColladaElement<IAsset>, IExtra
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Asset;

            public Contributor[] ContributorElements => GetChildren<Contributor>();
            public Coverage CoverageElement => GetChild<Coverage>();
            public Created CreatedElement => GetChild<Created>();
            public Keywords KeywordsElement => GetChild<Keywords>();
            public Modified ModifiedElement => GetChild<Modified>();
            public Revision RevisionElement => GetChild<Revision>();
            public Subject SubjectElement => GetChild<Subject>();
            public Title TitleElement => GetChild<Title>();
            public Unit UnitElement => GetChild<Unit>();
            public UpAxis UpAxisElement => GetChild<UpAxis>();
            public Extra[] ExtraElements => GetChildren<Extra>();

            #region Contributor
            [ElementName("contributor")]
            [Child(typeof(Author), 0, 1)]
            [Child(typeof(AuthorEmail), 0, 1)]
            [Child(typeof(AuthorWebsite), 0, 1)]
            [Child(typeof(AuthoringTool), 0, 1)]
            [Child(typeof(Comments), 0, 1)]
            [Child(typeof(Copyright), 0, 1)]
            [Child(typeof(SourceData), 0, 1)]
            public class Contributor : BaseColladaElement<Asset>
            {
                [ElementName("author")]
                public class Author : BaseStringElement<Contributor, ElementString> { }
                [ElementName("author_email")]
                public class AuthorEmail : BaseStringElement<Contributor, ElementString> { }
                [ElementName("author_website")]
                public class AuthorWebsite : BaseStringElement<Contributor, ElementString> { }
                [ElementName("authoring_tool")]
                public class AuthoringTool : BaseStringElement<Contributor, ElementString> { }
                [ElementName("comments")]
                public class Comments : BaseStringElement<Contributor, ElementString> { }
                [ElementName("copyright")]
                public class Copyright : BaseStringElement<Contributor, ElementString> { }
                [ElementName("source_data")]
                public class SourceData : BaseStringElement<Contributor, ElementString> { }
            }
            #endregion

            #region Coverage
            [ElementName("coverage")]
            [Child(typeof(GeographicLocation), 1)]
            public class Coverage : BaseColladaElement<Asset>
            {
                [ElementName("geographic_location")]
                [Child(typeof(Longitude), 1)]
                [Child(typeof(Latitude), 1)]
                [Child(typeof(Altitude), 1)]
                public class GeographicLocation : BaseColladaElement<Coverage>
                {
                    /// <summary>
                    /// -180.0f to 180.0f
                    /// </summary>
                    [ElementName("longitude")]
                    public class Longitude : BaseStringElement<GeographicLocation, StringPrimitive<float>> { }
                    /// <summary>
                    /// -90.0f to 90.0f
                    /// </summary>
                    [ElementName("latitude")]
                    public class Latitude : BaseStringElement<GeographicLocation, StringPrimitive<float>> { }
                    [ElementName("altitude")]
                    public class Altitude : BaseStringElement<GeographicLocation, StringPrimitive<float>>
                    {
                        public enum EMode
                        {
                            relativeToGround,
                            absolute,
                        }
                        [Attr("mode", true)]
                        public EMode Mode { get; set; }
                    }
                }
            }
            #endregion

            #region ElementChild Elements
            [ElementName("created")]
            public class Created : BaseStringElement<Asset, ElementString> { }
            [ElementName("keywords")]
            public class Keywords : BaseStringElement<Asset, ElementString> { }
            [ElementName("modified")]
            public class Modified : BaseStringElement<Asset, ElementString> { }
            [ElementName("revision")]
            public class Revision : BaseStringElement<Asset, ElementString> { }
            [ElementName("subject")]
            public class Subject : BaseStringElement<Asset, ElementString> { }
            [ElementName("title")]
            public class Title : BaseStringElement<Asset, ElementString> { }
            [ElementName("unit")]
            public class Unit : BaseColladaElement<Asset>
            {
                [Attr("meter", true)]
                [DefaultValue("1.0")]
                public Single Meter { get; set; }

                [Attr("Name", true)]
                [DefaultValue("meter")]
                public string Name { get; set; }

                //protected override bool WantsManualRead => true;
                //protected override void ManualReadAttribute(string elementName, string value)
                //{
                //    switch (elementName)
                //    {
                //        case "meter":
                //            Meter = Single.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                //            break;
                //        case "ElementName":
                //            elementName = value;
                //            break;
                //    }
                //}
            }
            public enum EUpAxis
            {
                /// <summary>
                /// Right: -Y, Up: +X, Backward: +Z
                /// </summary>
                X_UP,
                /// <summary>
                /// Right: +X, Up: +Y, Backward: +Z
                /// </summary>
                Y_UP,
                /// <summary>
                /// Right: +X, Up: +Z, Backward: -Y
                /// </summary>
                Z_UP,

                //To convert: move affected axes to proper spots.
                //Negate the original axis value if swapping into that spot and sign is different
            }
            [ElementName("up_axis")]
            public class UpAxis : BaseStringElement<Asset, StringPrimitive<EUpAxis>> { }
            #endregion
        }
        #endregion

        [ElementName("input")]
        public class InputUnshared : BaseColladaElement<IInputUnshared>
        {
            [Attr("semantic", true)]
            public string Semantic { get; set; }
            [Attr("source", true)]
            public ColladaURI Source { get; set; }

            public ESemantic CommonSemanticType
            {
                get => Semantic.AsEnum<ESemantic>();
                set => Semantic = value.ToString();
            }
        }
        [ElementName("input")]
        public class InputShared : BaseColladaElement<IInputShared>
        {
            [Attr("offset", true)]
            public uint Offset { get; set; }
            [Attr("set", false)]
            public uint Set { get; set; } = 0u;
            [Attr("semantic", true)]
            public string Semantic { get; set; }
            [Attr("source", true)]
            public ColladaURI Source { get; set; }

            public ESemantic CommonSemanticType
            {
                get => Semantic.AsEnum<ESemantic>();
                set => Semantic = value.ToString();
            }
        }
        [ElementName("technique")]
        public class Technique : BaseColladaElement<ITechnique>
        {
            [Attr("profile", true)]
            public string Profile { get; set; } = null;
            [Attr("xmlns", false)]
            public string XMLNS { get; set; } = null;
        }

        [ElementName("source")]
        [Child(typeof(Asset), 0, 1)]
        [Child(typeof(IArrayElement), 0, 1)]
        [Child(typeof(TechniqueCommon), 0, 1)]
        [UnsupportedChild("technique")]
        //[ElementChild(typeof(Technique), 0, -1)]
        public class Source : BaseColladaElement<ISource>, IID, IElementName, IAsset
        {
            public Asset AssetElement => GetChild<Asset>();
            public TechniqueCommon TechniqueCommonElement => GetChild<TechniqueCommon>();
            public T GetArrayElement<T>() where T : IArrayElement => GetChild<T>();

            [Attr("id", false)]
            public string ID { get; set; } = null;
            [Attr("Name", false)]
            public string Name { get; set; } = null;

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

            [ElementName("technique_common")]
            [Child(typeof(Accessor), 1)]
            public class TechniqueCommon : BaseColladaElement<Source>
            {
                public Accessor AccessorElement => GetChild<Accessor>();

                [ElementName("accessor")]
                [Child(typeof(DataFlowParam), 0, -1)]
                public class Accessor : BaseColladaElement<TechniqueCommon>, IDataFlowParam
                {
                    [Attr("count", true)]
                    public uint Count { get; set; } = 0;

                    [Attr("offset", false)]
                    [DefaultValue("0")]
                    public uint Offset { get; set; } = 0;

                    [Attr("source", true)]
                    public ColladaURI Source { get; set; } = null;

                    [Attr("stride", false)]
                    [DefaultValue("1")]
                    public uint Stride { get; set; } = 1;
                }
            }
            public class ArrayElement<T> :
                BaseStringElement<Source, T>, IID, IElementName, IArrayElement
                where T : BaseElementString
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("Name", false)]
                public string Name { get; set; } = null;
                [Attr("count", true)]
                public int Count { get; set; } = 0;

                public List<ISID> SIDElementChildren { get; } = new List<ISID>();
            }
            [ElementName("bool_array")]
            public class BoolArray : ArrayElement<ElementBoolArray> { }
            [ElementName("float_array")]
            public class FloatArray : ArrayElement<ElementFloatArray> { }
            [ElementName("int_array")]
            public class IntArray : ArrayElement<ElementIntArray> { }
            [ElementName("Name_array")]
            public class NameArray : ArrayElement<ElementStringArray> { }
            [ElementName("IDREF_array")]
            public class IDRefArray : ArrayElement<ElementStringArray> { }
            [ElementName("SIDREF_array")]
            public class SIDRefArray : ArrayElement<ElementStringArray> { }
            [ElementName("token_array")]
            public class TokenArray : ArrayElement<ElementStringArray> { }
        }

        #region Instance
        [Child(typeof(Extra), 0, -1)]
        public class BaseInstanceElement<T1, T2> : BaseColladaElement<T1>, ISID, IElementName, IExtra, IInstanceElement
            where T1 : class, IElement
            where T2 : class, IElement, IInstantiatable, IID
        {
            public Extra[] ExtraElements => GetChildren<Extra>();

            [Attr("sid", false)]
            public string SID { get; set; } = null;
            [Attr("Name", false)]
            public string Name { get; set; } = null;
            [Attr("url", true)]
            public ColladaURI Url { get; set; } = null;

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

            public T2 GetUrlInstance() => Url?.GetElement(Root as COLLADA) as T2;
        }
        [ElementName("instance_node")]
        public class InstanceNode : BaseInstanceElement<COLLADA.Node, COLLADA.Node>
        {
            [Attr("proxy", false)]
            public ColladaURI Proxy { get; set; } = null;

            public COLLADA.Node GetProxyInstance() => Proxy?.GetElement(Root as COLLADA) as COLLADA.Node;
        }

        [ElementName("instance_camera")]
        public class InstanceCamera : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryCameras.Camera>
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Cameras;
        }
        [ElementName("instance_light")]
        public class InstanceLight : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryLights.Light>
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Lights;
        }

        [ElementName("instance_geometry")]
        [Child(typeof(BindMaterial), 0, 1)]
        public class InstanceGeometry : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryGeometries.Geometry>, IInstanceMesh
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Geometry;

            public BindMaterial BindMaterialElement => GetChild<BindMaterial>();
        }
        [ElementName("instance_controller")]
        [Child(typeof(BindMaterial), 0, 1)]
        [Child(typeof(Skeleton), 0, -1)]
        public class InstanceController : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryControllers.Controller>, IInstanceMesh
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Controllers;

            public BindMaterial BindMaterialElement => GetChild<BindMaterial>();
            public Skeleton[] SkeletonElements => GetChildren<Skeleton>();

            [ElementName("skeleton")]
            public class Skeleton : BaseStringElement<InstanceController, ElementColladaURI> { }
        }
        [ElementName("bind_material")]
        [Child(typeof(DataFlowParam), 0, -1)]
        [Child(typeof(TechniqueCommon), 1)]
        [Child(typeof(Technique), 0, -1)]
        [Child(typeof(Extra), 0, -1)]
        public class BindMaterial : BaseColladaElement<IInstanceMesh>, ITechnique, IDataFlowParam
        {
            public Extra[] ExtraElements => GetChildren<Extra>();
            public DataFlowParam[] ParamElements => GetChildren<DataFlowParam>();
            public Technique[] TechniqueElements => GetChildren<Technique>();
            public TechniqueCommon TechniqueCommonElement => GetChild<TechniqueCommon>();

            [ElementName("technique_common")]
            [Child(typeof(InstanceMaterial), 1, -1)]
            public class TechniqueCommon : BaseColladaElement<BindMaterial>
            {
                public InstanceMaterial[] InstanceMaterialElements => GetChildren<InstanceMaterial>();

                [ElementName("instance_material")]
                public class InstanceMaterial : BaseColladaElement<TechniqueCommon>
                {
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;
                    [Attr("target", true)]
                    public ColladaURI Target { get; set; } = null;
                    [Attr("symbol", true)]
                    public string Symbol { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                }
            }
        }
        #endregion
    }
}

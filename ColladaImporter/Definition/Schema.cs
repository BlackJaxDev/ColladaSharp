using ColladaSharp.Transforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using XMLSchemaDefinition;

namespace ColladaSharp.Definition
{
    public partial class Collada
    {
        [ElementName("COLLADA")]
        [Child(typeof(Asset), 1)]
        [Child(typeof(Library), 0, -1)]
        [Child(typeof(Scene), 0, 1)]
        [Child(typeof(Extra), 0, -1)]
        public class COLLADA : BaseColladaElement<IElement>, IExtra, IAsset, IVersion, IRootElement
        {
            public Asset AssetElement => GetChild<Asset>();
            public T[] GetLibraries<T>() where T : Library => GetChildren<T>();
            public Scene SceneElement => GetChild<Scene>();
            public Extra[] ExtraElements => GetChildren<Extra>();

            [Attr("version", true)]
            [DefaultValue("1.5.0")]
            public string Version { get; set; }
            [Attr("xmlns", true)]
            [DefaultValue("https://collada.org/2008/03/COLLADASchema/")]
            public string Schema { get; set; }
            [Attr("base", false)]
            public string Base { get; set; }

            public Dictionary<string, List<IID>> IDEntries { get; } = new Dictionary<string, List<IID>>();
            public override List<IID> GetIDEntries(string id)
                => IDEntries.ContainsKey(id) ? IDEntries[id] : null;
            public T GetIDEntry<T>(string id)
            {
                List<T> entries = GetIDEntries(id).Where(x => x is T).Select(x => (T)x).ToList();
                if (entries.Count == 0)
                    return default;
                if (entries.Count == 1)
                    return entries[0];
                throw new InvalidOperationException();
            }

            #region Scene
            [ElementName("scene")]
            //[ElementChild(typeof(InstancePhysicsScene), 0, -1)]
            [UnsupportedChild("instance_physics_scene")]
            [Child(typeof(InstanceVisualScene), 0, 1)]
            [UnsupportedChild("instance_kinematics_scene")]
            //[ElementChild(typeof(InstanceKinematicsScene), 0, 1)]
            [Child(typeof(Extra), 0, -1)]
            public class Scene : BaseColladaElement<COLLADA>, IExtra
            {
                public Extra[] ExtraElements => GetChildren<Extra>();

                //[ElementName("instance_physics_scene")]
                //[ElementChild(typeof(Extra), 0, -1)]
                //public class InstancePhysicsScene : BaseColladaElement<Scene>, ISID, IElementName, IExtra
                //{
                //    [Attr("sid", false)]
                //    public string SID { get; set; } = null;
                //    [Attr("Name", false)]
                //    public string Name { get; set; } = null;
                //    [Attr("url", true)]
                //    public ColladaURI Url { get; set; } = null;

                //    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                //}
                [ElementName("instance_visual_scene")]
                [Child(typeof(Extra), 0, -1)]
                public class InstanceVisualScene : BaseColladaElement<Scene>
                {
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;
                    [Attr("url", true)]
                    public ColladaURI Url { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public LibraryVisualScenes.VisualScene GetUrlInstance() => Url.GetElement<LibraryVisualScenes.VisualScene>(Root);
                }
                //[ElementName("instance_kinematics_scene")]
                //[ElementChild(typeof(Asset), 0, 1)]
                //[ElementChild(typeof(NewParam), 0, -1)]
                //[ElementChild(typeof(SetParam), 0, -1)]
                ////[ElementChild(typeof(BindKinematicsModel), 0, -1)]
                ////[ElementChild(typeof(BindJointAxis), 0, -1)]
                //[ElementChild(typeof(Extra), 0, -1)]
                //public class InstanceKinematicsScene : BaseInstanceElement<Scene, LibraryKinematicsScenes.KinematicsScene>, ISID, IElementName, IExtra
                //{
                //    [Attr("sid", false)]
                //    public string SID { get; set; } = null;
                //    [Attr("Name", false)]
                //    public string Name { get; set; } = null;
                //    [Attr("url", true)]
                //    public ColladaURI Url { get; set; } = null;

                //    //TODO: BindKinematicsModel, BindJointAxis

                //    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                //}
            }

            #endregion

            #region Libraries
            [Child(typeof(Asset), 0, 1)]
            [Child(typeof(Extra), 0, -1)]
            public abstract class Library : BaseColladaElement<COLLADA>, IID, IElementName, IAsset, IExtra
            {
                public Asset AssetElement => GetChild<Asset>();
                public Extra[] ExtraElements => GetChildren<Extra>();

                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("Name", false)]
                public string Name { get; set; } = null;

                public List<ISID> SIDElementChildren { get; } = new List<ISID>();
            }

            public interface IImage { }

            #region Images
            [ElementName("library_images")]
            [Child(typeof(Image15X), 1, -1)]
            [Child(typeof(Image14X), 1, -1)]
            public class LibraryImages : Library
            {
                #region Image 1.5.*
                /// <summary>
                /// The <image> element best describes raster image data, but can conceivably handle other forms of
                /// imagery. Raster imagery data is typically organized in n-dimensional arrays. This array organization can be
                /// leveraged by texture look-up functions to access noncolor values such as displacement, normal, or height
                /// field values.
                /// </summary>
                [ElementName("image", "1.5.*")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(Renderable), 0, 1)]
                [Child(typeof(InitFrom), 0, 1)]
                //[ElementChild(typeof(Create2DEntry), 0, 1)]
                //[ElementChild(typeof(Create3DEntry), 0, 1)]
                //[ElementChild(typeof(CreateCubeEntry), 0, 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Image15X : BaseColladaElement<LibraryImages>, IID, ISID, IElementName, IImage
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Renderable RenderableElement => GetChild<Renderable>();
                    public InitFrom InitFromElement => GetChild<InitFrom>();
                    public Extra ExtraElements => GetChild<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    /// <summary>
                    /// Defines the image as a render target. If this element
                    /// exists then the image can be rendered to. 
                    /// This element contains no data. 
                    /// </summary>
                    [ElementName("renderable")]
                    public class Renderable : BaseColladaElement<Image15X>
                    {
                        /// <summary>
                        /// Set the required Boolean attribute share to true if,
                        /// when instantiated, the render target is to be shared
                        /// among all instances instead of being cloned.
                        /// </summary>
                        [Attr("share", false)]
                        [DefaultValue("true")]
                        public bool Share { get; set; } = true;
                    }

                    #region Init From
                    /// <summary>
                    /// Initializes the image from a URL (for example, a file) or a
                    /// list of hexadecimal values. Initialize the whole image
                    /// structure and data from formats such as DDS.
                    /// </summary>
                    [ElementName("init_from")]
                    [MultiChild(EMultiChildType.OneOfOne, typeof(Ref), typeof(Embedded))]
                    public class InitFrom : BaseColladaElement<Image15X>
                    {
                        public Ref RefElement => GetChild<Ref>();
                        public Embedded EmbeddedElement => GetChild<Embedded>();

                        /// <summary>
                        ///  Initializes higher MIP levels if data does not exist in a file. Defaults to true. 
                        /// </summary>
                        [Attr("mips_generate", false)]
                        [DefaultValue("true")]
                        public bool GenerateMipmaps { get; set; } = true;

                        /// <summary>
                        /// Contains the URL (xs:anyURI) of a file from which to take
                        /// initialization data. Assumes the characteristics of the file. If it
                        /// is a complex format such as DDS, this might include cube
                        /// maps, volumes, MIPs, and so on.
                        /// </summary>
                        [ElementName("ref")]
                        public class Ref : BaseStringElement<InitFrom, ElementString> { }
                        /// <summary>
                        /// Contains the embedded image data as a sequence of
                        /// hexadecimal-encoded binary octets. The data typically
                        /// contains all the necessary information including header info
                        /// such as data width and height.
                        /// </summary>
                        [ElementName("hex")]
                        public class Embedded : BaseStringElement<InitFrom, ElementHex>
                        {
                            /// <summary>
                            /// Use the required format attribute(xs:token) to specify which codec decodes the
                            /// image’s descriptions and data. This is usually its typical file
                            /// extension, such as BMP, JPG, DDS, TGA.
                            /// </summary>
                            [Attr("format", true)]
                            public string Format { get; set; }
                        }
                    }
                    #endregion

                    //TODO: finish create entries
                    #region Create
                    //[ElementName("create_2d")]
                    //private class Create2DEntry : BaseColladaElement<ImageEntry15X>
                    //{

                    //}
                    //[ElementName("create_3d")]
                    //private class Create3DEntry : BaseColladaElement<ImageEntry15X>
                    //{
                    //    [ElementName("init_from")]
                    //    private class InitFromCreate3DEntry : BaseColladaElement<Create3DEntry>
                    //    {
                    //        /// <summary>
                    //        /// Specifies which array element in the image to initialize (fill).
                    //        /// The default is 0. 
                    //        /// </summary>
                    //        [Attr("array_index", false)]
                    //        [DefaultValue("0")]
                    //        public uint ArrayIndex { get; set; } = 0u;

                    //        /// <summary>
                    //        /// Specifies which MIP level in the image to initialize. 
                    //        /// </summary>
                    //        [Attr("mip_index", true)]
                    //        public uint MipmapIndex { get; set; }

                    //        /// <summary>
                    //        /// Required in <create_3d>; not valid in <create_2d> or <create_cube>. 
                    //        /// </summary>
                    //        [Attr("depth", true)]
                    //        public uint Depth { get; set; }
                    //    }
                    //}
                    //[ElementName("create_cube")]
                    //private class CreateCubeEntry : BaseColladaElement<ImageEntry15X>
                    //{

                    //}
                    #endregion
                }
                #endregion

                #region Image 1.4.*
                /// <summary>
                /// The <image> element best describes raster image data, but can conceivably handle other forms of
                /// imagery. Raster imagery data is typically organized in n-dimensional arrays. This array organization can be
                /// leveraged by texture look-up functions to access noncolor values such as displacement, normal, or height
                /// field values.
                /// </summary>
                [ElementName("image", "1.4.*")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(ISource), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Image14X : BaseColladaElement<LibraryImages>, IID, ISID, IElementName, IImage
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    [Attr("format", false)]
                    public string Format { get; set; } = null;
                    [Attr("height", false)]
                    public uint? Height { get; set; } = null;
                    [Attr("width", false)]
                    public uint? Width { get; set; } = null;
                    [Attr("depth", false)]
                    public uint? Depth { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public interface ISource : IElement { }

                    [ElementName("init_from")]
                    public class InitFrom : BaseStringElement<Image14X, ElementString>, ISource { }
                    [ElementName("data")]
                    public class Data : BaseStringElement<Image14X, ElementHex>, ISource { }
                }
                #endregion
            }
            #endregion

            #region Materials
            [ElementName("library_materials")]
            [Child(typeof(Material), 1, -1)]
            public class LibraryMaterials : Library
            {
                [ElementName("material")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(InstanceEffect), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Material : BaseColladaElement<LibraryMaterials>, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public InstanceEffect InstanceEffectElement => GetChild<InstanceEffect>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    [ElementName("instance_effect")]
                    //[ElementChild(typeof(TechniqueHint), 0, -1)]
                    //[ElementChild(typeof(SetParam), 0, -1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class InstanceEffect : BaseColladaElement<Material>, ISID, IElementName, IExtra
                    {
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("sid", false)]
                        public string SID { get; set; } = null;
                        [Attr("Name", false)]
                        public string Name { get; set; } = null;
                        [Attr("url", true)]
                        public ColladaURI Url { get; set; } = null;

                        public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                        //TODO: TechniqueHint
                    }
                }
            }
            #endregion

            #region Effects
            [ElementName("library_effects")]
            [Child(typeof(Effect), 1, -1)]
            public class LibraryEffects : Library
            {
                [ElementName("effect")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(Annotate), 0, -1)]
                [Child(typeof(NewParam), 0, -1)]
                [Child(typeof(BaseProfile), 1, -1)]
                [Child(typeof(Extra), 0, -1)]
                public class Effect : BaseColladaElement<LibraryEffects>, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Annotate[] AnnotateElements => GetChildren<Annotate>();
                    public NewParam[] NewParamElements => GetChildren<NewParam>();
                    public BaseProfile[] ProfileElements => GetChildren<BaseProfile>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    [Child(typeof(Asset), 0, 1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class BaseProfile : BaseColladaElement<Effect>, IID, IAsset, IExtra
                    {
                        public Asset AssetElement => GetChild<Asset>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("id", false)]
                        public string ID { get; set; } = null;

                        public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                    }
                    [Child(typeof(Technique), 1)]
                    public class BaseProfileShader : BaseProfile
                    {
                        /// <summary>
                        /// The type of platform. This is a vendor-defined character string that indicates the
                        /// platform or capability target for the technique. The default is “PC”. 
                        /// </summary>
                        [Attr("platform", false)]
                        [DefaultValue("PC")]
                        public string Platform { get; set; } = "PC";

                        [ElementName("technique")]
                        [Child(typeof(Asset), 0, 1)]
                        [Child(typeof(Annotate), 0, -1)]
                        [Child(typeof(Pass), 1, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Technique : BaseColladaElement<BaseProfileShader>, IID, ISID, IAsset, IExtra
                        {
                            public Asset AssetElement => GetChild<Asset>();
                            public Annotate[] AnnotateElements => GetChildren<Annotate>();
                            public Pass[] PassElements => GetChildren<Pass>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("sid", false)]
                            public string SID { get; set; } = null;

                            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                            [ElementName("pass")]
                            public class Pass : BaseColladaElement<Technique>
                            {
                                //TODO
                            }
                        }
                    }

                    #region Profile Common
                    [ElementName("profile_COMMON")]
                    [Child(typeof(NewParam), 0, -1)]
                    [Child(typeof(Technique), 1)]
                    public class ProfileCommon : BaseProfile
                    {
                        public Technique TechniqueElement => GetChild<Technique>();
                        public NewParam NewParamElement => GetChild<NewParam>();

                        [ElementName("technique")]
                        [Child(typeof(BaseTechniqueElementChild), 1)]
                        public class Technique : BaseColladaElement<ProfileCommon>, IID, ISID
                        {
                            public BaseTechniqueElementChild LightingTypeElement => GetChild<BaseTechniqueElementChild>();

                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("sid", false)]
                            public string SID { get; set; } = null;

                            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                            public enum EOpaque
                            {
                                /// <summary>
                                ///  (the default): Takes the transparency information from the color’s
                                ///  alpha channel, where the value 1.0 is opaque.
                                /// </summary>
                                A_ONE,
                                /// <summary>
                                /// Takes the transparency information from the color’s red, green,
                                /// and blue channels, where the value 0.0 is opaque, with each channel 
                                /// modulated independently.
                                /// </summary>
                                RGB_ZERO,
                                /// <summary>
                                /// Takes the transparency information from the color’s
                                /// alpha channel, where the value 0.0 is opaque.
                                /// </summary>
                                A_ZERO,
                                /// <summary>
                                ///  Takes the transparency information from the color’s red, green,
                                ///  and blue channels, where the value 1.0 is opaque, with each channel 
                                ///  modulated independently.
                                /// </summary>
                                RGB_ONE,
                            }

                            public class BaseTechniqueElementChild : BaseColladaElement<Technique> { }

                            [MultiChild(EMultiChildType.OneOfOne, typeof(Color), typeof(RefParam), typeof(Texture))]
                            public class BaseFXColorTexture : BaseColladaElement<BaseTechniqueElementChild>, IRefParam
                            {
                                public Color ColorElement => GetChild<Color>();
                                public RefParam ParamElement => GetChild<RefParam>();
                                public Texture TextureElement => GetChild<Texture>();

                                [ElementName("color")]
                                public class Color : BaseStringElement<BaseFXColorTexture, StringParsable<Vec4>>, ISID
                                {
                                    [Attr("sid", false)]
                                    public string SID { get; set; } = null;

                                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                                }
                                [ElementName("texture")]
                                [Child(typeof(Extra), 0, -1)]
                                public class Texture : BaseColladaElement<BaseFXColorTexture>, IExtra
                                {
                                    public Extra[] ExtraElements => GetChildren<Extra>();

                                    [Attr("texture", true)]
                                    public string TextureID { get; set; }
                                    [Attr("texcoord", true)]
                                    public string TexcoordID { get; set; }
                                }
                            }

                            [MultiChild(EMultiChildType.OneOfOne, typeof(Float), typeof(RefParam))]
                            public class BaseFXFloatParam : BaseColladaElement<BaseTechniqueElementChild>, IRefParam
                            {
                                [ElementName("float")]
                                public class Float : BaseStringElement<BaseFXFloatParam, StringPrimitive<float>>, ISID
                                {
                                    [Attr("sid", false)]
                                    public string SID { get; set; } = null;

                                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                                }
                            }

                            /// <summary>
                            /// Declares the amount of light emitted from the surface of this object. 
                            /// </summary>
                            [ElementName("emission")]
                            public class Emission : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of ambient light reflected from the surface of this object. 
                            /// </summary>
                            [ElementName("ambient")]
                            public class Ambient : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of light diffusely reflected from the surface of this object. 
                            /// </summary>
                            [ElementName("diffuse")]
                            public class Diffuse : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the color of light specularly reflected from the surface of this object. 
                            /// </summary>
                            [ElementName("specular")]
                            public class Specular : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the specularity or roughness of the specular reflection lobe.
                            /// </summary>
                            [ElementName("shininess")]
                            public class Shininess : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the color of a perfect mirror reflection. 
                            /// </summary>
                            [ElementName("reflective")]
                            public class Reflective : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of perfect mirror reflection to be
                            /// added to the reflected light as a value between 0.0 and 1.0.
                            /// </summary>
                            [ElementName("reflectivity")]
                            public class Reflectivity : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the color of perfectly refracted light. 
                            /// </summary>
                            [ElementName("transparent")]
                            public class Transparent : BaseFXColorTexture
                            {
                                [Attr("opaque", false)]
                                [DefaultValue("A_ONE")]
                                public EOpaque Opaque { get; set; } = EOpaque.A_ONE;
                            }
                            /// <summary>
                            /// Declares the amount of perfectly refracted light added
                            /// to the reflected color as a scalar value between 0.0 and 1.0. 
                            /// </summary>
                            [ElementName("transparency")]
                            public class Transparency : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the index of refraction for perfectly refracted
                            /// light as a single scalar index.
                            /// </summary>
                            [ElementName("index_of_refraction")]
                            public class IndexOfRefraction : BaseFXFloatParam { }

                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, 
                            /// declares a fixed-function pipeline that produces a constantly
                            /// shaded surface that is independent of lighting.
                            /// The reflected color is calculated as color = emission + ambient * al
                            /// 'al' is a constant amount of ambient light contribution coming from the scene.
                            /// In the COMMON profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            /// </summary>
                            [ElementName("constant")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Constant : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, declares a fixed-function pipeline that produces a diffuse
                            ///shaded surface that is independent of lighting.
                            ///The result is based on Lambert’s Law, which states that when light hits a rough surface, the light is
                            ///reflected in all directions equally.The reflected color is calculated simply as:
                            /// color = emission + ambient * al + diffuse * max(N*L, 0)
                            /// where:
                            ///• al – A constant amount of ambient light contribution coming from the scene. In the COMMON
                            /// profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            ///• N – Normal vector
                            ///• L – Light vector
                            /// </summary>
                            [ElementName("lambert")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Ambient), 0, 1)]
                            [Child(typeof(Diffuse), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Lambert : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Ambient AmbientElement => GetChild<Ambient>();
                                public Diffuse DiffuseElement => GetChild<Diffuse>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, declares a fixed-function pipeline that produces a specularly
                            /// shaded surface that reflects ambient, diffuse, and specular reflection, where the specular reflection is
                            /// shaded according the Phong BRDF approximation.
                            /// The <phong> shader uses the common Phong shading equation, that is:
                            /// color = emission + ambient * al + diffuse * max(N * L, 0) + specular * max(R * I, 0)^shininess
                            /// where:
                            /// • al – A constant amount of ambient light contribution coming from the scene.
                            /// In the COMMON profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            /// • N – Normal vector
                            /// • L – Light vector
                            /// • I – Eye vector
                            /// • R – Perfect reflection vector (reflect (L around N)) 
                            /// </summary>
                            [ElementName("phong")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Ambient), 0, 1)]
                            [Child(typeof(Diffuse), 0, 1)]
                            [Child(typeof(Specular), 0, 1)]
                            [Child(typeof(Shininess), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Phong : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Ambient AmbientElement => GetChild<Ambient>();
                                public Diffuse DiffuseElement => GetChild<Diffuse>();
                                public Specular SpecularElement => GetChild<Specular>();
                                public Shininess ShininessElement => GetChild<Shininess>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, <blinn> declares a fixed-function pipeline that produces a
                            /// shaded surface according to the Blinn-Torrance-Sparrow lighting model or a close approximation.
                            /// This equation is complex and detailed via the ACM, so it is not detailed here.Refer to “Models of Light
                            /// Reflection for Computer Synthesized Pictures,” SIGGRAPH 77, pp 192-198
                            /// (http://portal.acm.org/citation.cfm?id=563893).
                            /// Maximizing Compatibility:
                            /// To maximize application compatibility, it is suggested that developers use the Blinn-Torrance-Sparrow for
                            /// <shininess> in the range of 0 to 1. For<shininess> greater than 1.0, the COLLADA author was
                            /// probably using an application that followed the Blinn-Phong lighting model, so it is recommended to
                            /// support both Blinn equations according to whichever range the shininess resides in.
                            /// The Blinn-Phong equation
                            /// The Blinn-Phong equation is:
                            /// color = emission + ambient * al + diffuse * max(N * L, 0) + specular * max(H * N, 0)^shininess
                            /// where:
                            /// • al – A constant amount of ambient light contribution coming from the scene.In the COMMON
                            /// profile, this is the sum of all the <light><technique_common><ambient> values in the <visual_scene>.
                            /// • N – Normal vector (normalized)
                            /// • L – Light vector (normalized)
                            /// • I – Eye vector (normalized)
                            /// • H – Half-angle vector, calculated as halfway between the unit Eye and Light vectors, using the equation H = normalize(I + L)
                            /// </summary>
                            [ElementName("blinn")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Ambient), 0, 1)]
                            [Child(typeof(Diffuse), 0, 1)]
                            [Child(typeof(Specular), 0, 1)]
                            [Child(typeof(Shininess), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Blinn : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Ambient AmbientElement => GetChild<Ambient>();
                                public Diffuse DiffuseElement => GetChild<Diffuse>();
                                public Specular SpecularElement => GetChild<Specular>();
                                public Shininess ShininessElement => GetChild<Shininess>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
                        }
                    }
                    #endregion

                    [ElementName("profile_GLSL")]
                    public class ProfileGLSL : BaseProfileShader
                    {

                    }
                }
            }
            #endregion

            #region Cameras
            [ElementName("library_cameras")]
            [Child(typeof(Camera), 1, -1)]
            public class LibraryCameras : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Cameras;

                [ElementName("camera")]
                public class Camera : BaseColladaElement<LibraryCameras>, IInstantiatable, IID
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                }
            }
            #endregion

            #region Geometry
            [ElementName("library_geometries")]
            [Child(typeof(Geometry), 1, -1)]
            public class LibraryGeometries : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Geometry;

                [ElementName("geometry")]
                [Child(typeof(Asset), 0, 1)]
                [UnsupportedChild("convex_mesh")]
                [UnsupportedChild("spline")]
                [UnsupportedChild("brep")]
                [Child(typeof(GeometryElementChild), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Geometry : BaseColladaElement<LibraryGeometries>, IInstantiatable, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public ConvexMesh ConvexMeshElement => GetChild<ConvexMesh>();
                    public Mesh MeshElement => GetChild<Mesh>();
                    public Spline SplineElement => GetChild<Spline>();
                    public BRep BRepElement => GetChild<BRep>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public class GeometryElementChild : BaseColladaElement<Geometry> { }

                    [ElementName("convex_mesh")]
                    public class ConvexMesh : GeometryElementChild
                    {

                    }
                    [ElementName("mesh")]
                    [Child(typeof(Source), 1, -1)]
                    [Child(typeof(Vertices), 1)]
                    [Child(typeof(BasePrimitive), 0, -1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class Mesh : GeometryElementChild, ISource
                    {
                        public Source[] SourceElements => GetChildren<Source>();
                        public Vertices VerticesElement => GetChild<Vertices>();
                        public BasePrimitive[] PrimitiveElements => GetChildren<BasePrimitive>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [ElementName("vertices")]
                        [Child(typeof(InputUnshared), 1, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Vertices : BaseColladaElement<Mesh>, IID, IElementName, IInputUnshared
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("Name", false)]
                            public string Name { get; set; } = null;

                            public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                        }
                        [Child(typeof(InputShared), 0, -1)]
                        [Child(typeof(Indices), 0, 1)]
                        [Child(typeof(Extra), 0, -1)]
                        public abstract class BasePrimitive : BaseColladaElement<Mesh>, IElementName, IExtra, IInputShared
                        {
                            public InputShared[] InputElements => GetChildren<InputShared>();
                            public Indices IndicesElement => GetChild<Indices>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("Name", false)]
                            public string Name { get; set; } = null;
                            [Attr("count", true)]
                            public int Count { get; set; } = 0;
                            [Attr("material", false)]
                            public ColladaURI Material { get; set; } = null;

                            public int PointCount { get; set; }
                            public int FaceCount { get; private set; }
                            public abstract EColladaPrimitiveType Type { get; }

                            protected override void PostRead()
                            {
                                PointCount = IndicesElement.StringContent.Values.Length / InputElements.Length;
                                switch (Type)
                                {
                                    case EColladaPrimitiveType.Trifans:
                                    case EColladaPrimitiveType.Tristrips:
                                    case EColladaPrimitiveType.Polygons:
                                    case EColladaPrimitiveType.Polylist:
                                        FaceCount = PointCount - 2;
                                        break;
                                    case EColladaPrimitiveType.Triangles:
                                        FaceCount = PointCount / 3;
                                        break;
                                    case EColladaPrimitiveType.Lines:
                                        FaceCount = PointCount / 2;
                                        break;
                                    case EColladaPrimitiveType.Linestrips:
                                        FaceCount = PointCount - 1;
                                        break;
                                }
                            }

                            [ElementName("p")]
                            public class Indices : BaseStringElement<BasePrimitive, ElementIntArray> { }
                        }
                        public enum EColladaPrimitiveType
                        {
                            Lines,
                            Linestrips,
                            Polygons,
                            Polylist,
                            Triangles,
                            Trifans,
                            Tristrips,
                        }
                        [ElementName("lines")]
                        public class Lines : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Lines;
                        }
                        [ElementName("linestrips")]
                        public class Linestrips : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Linestrips;
                        }
                        [ElementName("polygons")]
                        public class Polygons : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Polygons;
                        }
                        [ElementName("polylist")]
                        [Child(typeof(PolyCounts), 0, 1)]
                        public class Polylist : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Polylist;

                            public PolyCounts PolyCountsElement => GetChild<PolyCounts>();

                            [ElementName("vcount")]
                            public class PolyCounts : BaseStringElement<BasePrimitive, ElementIntArray> { }
                        }
                        [ElementName("triangles")]
                        public class Triangles : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Triangles;
                        }
                        [ElementName("trifans")]
                        public class Trifans : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Trifans;
                        }
                        [ElementName("tristrips")]
                        public class Tristrips : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Tristrips;
                        }
                    }
                    [ElementName("spline")]
                    public class Spline : GeometryElementChild
                    {

                    }
                    [ElementName("brep")]
                    public class BRep : GeometryElementChild
                    {

                    }
                }
            }
            #endregion

            #region Controllers
            [ElementName("library_controllers")]
            [Child(typeof(Controller), 1, -1)]
            public class LibraryControllers : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Controllers;

                [ElementName("controller")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(ControllerElementChild), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Controller : BaseColladaElement<LibraryControllers>, IInstantiatable, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public ControllerElementChild SkinOrMorphElement => GetChild<ControllerElementChild>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public abstract class ControllerElementChild : BaseColladaElement<Controller> { }
                    [ElementName("skin")]
                    [Child(typeof(BindShapeMatrix), 0, 1)]
                    [Child(typeof(Source), 3, -1)]
                    [Child(typeof(Joints), 1)]
                    [Child(typeof(VertexWeights), 1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class Skin : ControllerElementChild, ISource, IExtra
                    {
                        public BindShapeMatrix BindShapeMatrixElement => GetChild<BindShapeMatrix>();
                        public Source[] SourceElements => GetChildren<Source>();
                        public Joints JointsElement => GetChild<Joints>();
                        public VertexWeights VertexWeightsElement => GetChild<VertexWeights>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("source", true)]
                        public ColladaURI Source { get; set; }

                        [ElementName("bind_shape_matrix")]
                        public class BindShapeMatrix : BaseStringElement<Skin, StringParsable<Matrix4>>
                        {
                            protected override void PostRead() => StringContent.Value = StringContent.Value.Transposed();
                        }
                        [ElementName("joints")]
                        [Child(typeof(InputUnshared), 2, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Joints : BaseColladaElement<Skin>, IInputUnshared, IExtra
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                        }
                        [ElementName("vertex_weights")]
                        [Child(typeof(InputShared), 2, -1)]
                        [Child(typeof(BoneCounts), 0, 1)]
                        [Child(typeof(PrimitiveIndices), 0, 1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class VertexWeights : BaseColladaElement<Skin>, IInputShared, IExtra
                        {
                            public InputShared[] InputElements => GetChildren<InputShared>();
                            public BoneCounts BoneCountsElement => GetChild<BoneCounts>();
                            public PrimitiveIndices PrimitiveIndicesElement => GetChild<PrimitiveIndices>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("count", true)]
                            public uint Count { get; set; } = 0;

                            [ElementName("vcount")]
                            public class BoneCounts : BaseStringElement<VertexWeights, ElementIntArray> { }
                            [ElementName("v")]
                            public class PrimitiveIndices : BaseStringElement<VertexWeights, ElementIntArray> { }
                        }
                    }
                    public enum EMorphMethod
                    {
                        NORMALIZED,
                        RELATIVE,
                    }
                    [ElementName("morph")]
                    [Child(typeof(Source), 2, -1)]
                    [Child(typeof(Targets), 1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class Morph : ControllerElementChild, ISource, IExtra
                    {
                        public Source[] SourceElements => GetChildren<Source>();
                        public Targets TargetsElement => GetChild<Targets>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("source", true)]
                        public ColladaURI BaseMeshUrl { get; set; }
                        [Attr("method", false)]
                        [DefaultValue("NORMALIZED")]
                        public EMorphMethod Method { get; set; } = EMorphMethod.NORMALIZED;

                        [ElementName("targets")]
                        [Child(typeof(InputUnshared), 2, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Targets : BaseColladaElement<Morph>, IInputUnshared, IExtra
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                        }
                    }
                }
            }
            #endregion

            #region Lights
            [ElementName("library_lights")]
            [Child(typeof(Light), 1, -1)]
            public class LibraryLights : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Lights;

                [ElementName("light")]
                public class Light : BaseColladaElement<LibraryLights>, IInstantiatable, IID
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                }
            }
            #endregion

            #region Animation
            public interface IAnimation : IElement { }
            [ElementName("library_animations")]
            [Child(typeof(Animation), 1, -1)]
            public class LibraryAnimations : Library, IAnimation
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Animations;

                public Animation[] AnimationElements => GetChildren<Animation>();

                [ElementName("animation")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(Animation), 0, -1)]
                [Child(typeof(Source), 0, -1)]
                [Child(typeof(Sampler), 0, -1)]
                [Child(typeof(Channel), 0, -1)]
                [Child(typeof(Extra), 0, -1)]
                public class Animation : BaseColladaElement<IAnimation>, IID, IElementName, IAnimation, IAsset, ISource, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Animation[] AnimationElements => GetChildren<Animation>();
                    public Source[] SourceElements => GetChildren<Source>();
                    public Sampler[] SamplerElements => GetChildren<Sampler>();
                    public Channel[] ChannelElements => GetChildren<Channel>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public enum EInterpBehavior
                    {
                        UNDEFINED,
                        CONSTANT,
                        GRADIENT,
                        CYCLE,
                        OSCILLATE,
                        CYCLE_RELATIVE,
                    }

                    [ElementName("sampler")]
                    [Child(typeof(InputUnshared), 1, -1)]
                    public class Sampler : BaseColladaElement<Animation>, IID, IInputUnshared
                    {
                        public InputUnshared[] InputElements => GetChildren<InputUnshared>();

                        [Attr("id", false)]
                        public string ID { get; set; } = null;
                        [Attr("pre_behavior", false)]
                        public EInterpBehavior PreBehavior { get; set; }
                        [Attr("post_behavior", false)]
                        public EInterpBehavior PostBehavior { get; set; }

                        public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                    }
                    [ElementName("channel")]
                    public class Channel : BaseColladaElement<Animation>
                    {
                        public enum ESelector
                        {
                            /// <summary>
                            /// Red color component
                            /// </summary>
                            R,
                            /// <summary>
                            /// Green color component
                            /// </summary>
                            G,
                            /// <summary>
                            /// Blue color component
                            /// </summary>
                            B,
                            /// <summary>
                            /// Alpha color component
                            /// </summary>
                            A,
                            /// <summary>
                            /// First cartesian coordinate
                            /// </summary>
                            X,
                            /// <summary>
                            /// Second cartesian coordinate
                            /// </summary>
                            Y,
                            /// <summary>
                            /// Third cartesian coordinate
                            /// </summary>
                            Z,
                            /// <summary>
                            /// Fourth cartesian coordinate
                            /// </summary>
                            W,
                            /// <summary>
                            /// First texture coordinate
                            /// </summary>
                            S,
                            /// <summary>
                            /// Second texture coordinate
                            /// </summary>
                            T,
                            /// <summary>
                            /// Third texture coordinate
                            /// </summary>
                            P,
                            /// <summary>
                            /// Fourth texture coordinate
                            /// </summary>
                            Q,
                            /// <summary>
                            /// First generic parameter
                            /// </summary>
                            U,
                            /// <summary>
                            /// Second generic parameter
                            /// </summary>
                            V,
                            /// <summary>
                            /// Axis-angle angle
                            /// </summary>
                            ANGLE,
                            /// <summary>
                            /// Time in seconds
                            /// </summary>
                            TIME,
                        }
                        [Attr("source", true)]
                        public ColladaURI Source { get; set; }
                        [Attr("target", true)]
                        public SidRef Target { get; set; }
                    }
                }
            }
            #endregion

            #region Nodes
            /// <summary>
            /// Indicates that this class owns Node elements.
            /// </summary>
            public interface INode : IElement
            {
                Node[] NodeElements { get; }
            }
            [ElementName("node")]
            [Child(typeof(Asset), 0, 1)]
            [Child(typeof(ITransformation), 0, -1)]
            [UnsupportedChild("lookat")]
            [UnsupportedChild("skew")]
            [Child(typeof(IInstanceElement), 0, -1)]
            [Child(typeof(Node), 0, -1)]
            [Child(typeof(Extra), 0, -1)]
            public class Node : BaseColladaElement<INode>,
                INode, IID, ISID, IElementName, IInstantiatable, IExtra, IAsset
            {
                public Asset AssetElement => GetChild<Asset>();
                public Extra[] ExtraElements => GetChildren<Extra>();
                public ITransformation[] TransformElements => GetChildren<ITransformation>();
                public IInstanceElement[] InstanceElements => GetChildren<IInstanceElement>();
                public Node[] NodeElements => GetChildren<Node>();

                public enum EType
                {
                    NODE,
                    JOINT,
                }

                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("Name", false)]
                public string Name { get; set; } = null;
                [Attr("type", false)]
                [DefaultValue("NODE")]
                public EType Type { get; set; } = EType.NODE;
                [Attr("layer", false)]
                public string Layer { get; set; } = null;

                public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                public Node FindNode(string sid) => FindNode(NodeElements, sid);
                private static Node FindNode(Node[] nodes, string sid)
                {
                    foreach (var n1 in nodes)
                    {
                        if (n1.SID == sid)
                            return n1;
                        var n2 = n1.FindNode(sid);
                        if (n2 != null)
                            return n2;
                    }
                    return null;
                }

                public interface ITransformation : IElement
                {
                    Matrix4 GetMatrix();
                }
                public abstract class Transformation<T> : BaseStringElement<Node, StringParsable<T>>, ISID, ITransformation where T : IParsable
                {
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public abstract Matrix4 GetMatrix();
                }
                [ElementName("translate")]
                public class Translate : Transformation<Vec3>
                {
                    public override Matrix4 GetMatrix()
                        => Matrix4.CreateTranslation(StringContent.Value);
                }
                [ElementName("scale")]
                public class Scale : Transformation<Vec3>
                {
                    public override Matrix4 GetMatrix()
                        => Matrix4.CreateScale(StringContent.Value);
                }
                [ElementName("rotate")]
                public class Rotate : Transformation<Vec4>
                {
                    public override Matrix4 GetMatrix()
                        => Matrix4.CreateFromAxisAngleRad(StringContent.Value.Xyz, StringContent.Value.W);
                }
                [ElementName("matrix")]
                public class Matrix : Transformation<Matrix4>
                {
                    protected override void PostRead()
                        => StringContent.Value = StringContent.Value.Transposed();
                    public override Matrix4 GetMatrix()
                        => StringContent.Value;
                }

                public Matrix4 GetTransformMatrix()
                {
                    Matrix4 m = Matrix4.Identity;
                    foreach (ITransformation t in TransformElements)
                        m = m * t.GetMatrix();
                    return m;
                }

                //public override bool WantsManualRead => true;
                //public override void SetAttribute(string ElementName, string value)
                //{
                //    switch (ElementName)
                //    {
                //        case "id":
                //            ID = value;
                //            break;
                //        case "sid":
                //            SID = value;
                //            break;
                //        case "ElementName":
                //            ElementName = value;
                //            break;
                //        case "type":
                //            Type = value == "JOINT" ? EType.JOINT : EType.NODE;
                //            break;
                //        case "layer":
                //            Layer = value;
                //            break;
                //    }
                //}
                //public override IElement CreateElementChildElement(string ElementName, string version)
                //{
                //    switch (ElementName)
                //    {
                //        case "asset":
                //            return new Asset();
                //        case "extra":
                //            return new Extra();
                //        case "node":
                //            return new Node();
                //        case "translate":
                //            return new Translate();
                //        case "scale":
                //            return new Scale();
                //        case "rotate":
                //            return new Rotate();
                //        case "matrix":
                //            return new Matrix();
                //        case "instance_node":
                //            return new InstanceNode();
                //        case "instance_camera":
                //            return new InstanceCamera();
                //        case "instance_controller":
                //            return new InstanceController();
                //        case "instance_geometry":
                //            return new InstanceGeometry();
                //        case "instance_light":
                //            return new InstanceLight();
                //    }
                //    return null;
                //}
            }
            #endregion

            #region Visual Scenes
            [ElementName("library_visual_scenes")]
            [Child(typeof(VisualScene), 1, -1)]
            public class LibraryVisualScenes : Library
            {
                [ElementName("visual_scene")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(Node), 1, -1)]
                //[ElementChild(typeof(EvaluateScene), 0, -1)]
                [UnsupportedChild("evaluate_scene")]
                [Child(typeof(Extra), 0, -1)]
                public class VisualScene : BaseColladaElement<LibraryVisualScenes>,
                    IAsset, IExtra, INode, IID, IElementName, IInstantiatable
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Node[] NodeElements => GetChildren<Node>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public Node FindNode(string sid) => FindNode(NodeElements, sid);
                    private static Node FindNode(Node[] nodes, string sid)
                    {
                        foreach (var n1 in nodes)
                        {
                            if (n1.SID == sid)
                                return n1;
                            var n2 = n1.FindNode(sid);
                            if (n2 != null)
                                return n2;
                        }
                        return null;
                    }

                    [ElementName("evaluate_scene")]
                    public class EvaluateScene : BaseColladaElement<VisualScene>
                    {
                        //TODO
                    }
                }
            }
            #endregion

            #endregion

            //protected override bool WantsManualRead => true;
            //protected override void ManualReadAttribute(string elementName, string value)
            //{
            //    switch (elementName)
            //    {
            //        case "version":
            //            Version = value;
            //            break;
            //        case "xmlns":
            //            Schema = value;
            //            break;
            //        case "base":
            //            Base = value;
            //            break;
            //    }
            //}
            //protected override IElement ManualReadChildElement(string elementName, string version)
            //{
            //    switch (elementName)
            //    {
            //        case "asset":
            //            return new Asset();
            //        case "extra":
            //            return new Extra();
            //        case "library_images":
            //            return new LibraryImages();
            //        case "library_materials":
            //            return new LibraryMaterials();
            //        case "library_effects":
            //            return new LibraryEffects();
            //        case "library_geometries":
            //            return new LibraryGeometries();
            //        case "library_controllers":
            //            return new LibraryControllers();
            //        case "library_visual_scenes":
            //            return new LibraryVisualScenes();
            //        case "library_animations":
            //            return new LibraryAnimations();
            //        case "library_cameras":
            //            return new LibraryCameras();
            //        case "scene":
            //            return new Scene();
            //    }
            //    return null;
            //}
        }

        public enum ESemantic
        {
            /// <summary>
            /// Semantic type is not defined in this list.
            /// </summary>
            UNDEFINED,
            /// <summary>
            /// Geometric binormal (bitangent) vector
            /// </summary>
            BINORMAL,
            /// <summary>
            /// Color coordinate vector. Color inputs are RGB (float3)
            /// </summary>
            COLOR,
            /// <summary>
            /// Continuity constraint at the control vertex (CV).
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            CONTINUITY,
            /// <summary>
            /// Raster or MIP-level input.
            /// </summary>
            IMAGE,
            /// <summary>
            /// Sampler input.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            INPUT,
            /// <summary>
            /// Tangent vector for preceding control point.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            IN_TANGENT,
            /// <summary>
            /// Sampler interpolation type.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            INTERPOLATION,
            /// <summary>
            /// Inverse of local-to-world matrix.
            /// </summary>
            INV_BIND_MATRIX,
            /// <summary>
            /// Skin influence identifier
            /// </summary>
            JOINT,
            /// <summary>
            /// Number of piece-wise linear approximation steps to use for the spline segment that follows this CV.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            LINEAR_STEPS,
            /// <summary>
            /// Morph targets for mesh morphing
            /// </summary>
            MORPH_TARGET,
            /// <summary>
            /// Weights for mesh morphing
            /// </summary>
            MORPH_WEIGHT,
            /// <summary>
            /// Normal vector
            /// </summary>
            NORMAL,
            /// <summary>
            /// Sampler output.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            OUTPUT,
            /// <summary>
            /// Tangent vector for succeeding control point.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            OUT_TANGENT,
            /// <summary>
            /// Geometric coordinate vector. See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            POSITION,
            /// <summary>
            /// Geometric tangent vector
            /// </summary>
            TANGENT,
            /// <summary>
            /// Texture binormal (bitangent) vector
            /// </summary>
            TEXBINORMAL,
            /// <summary>
            /// Texture coordinate vector
            /// </summary>
            TEXCOORD,
            /// <summary>
            /// Texture tangent vector
            /// </summary>
            TEXTANGENT,
            /// <summary>
            /// Generic parameter vector
            /// </summary>
            UV,
            /// <summary>
            /// Mesh vertex
            /// </summary>
            VERTEX,
            /// <summary>
            /// Skin influence weighting value
            /// </summary>
            WEIGHT,
        }
    }
}

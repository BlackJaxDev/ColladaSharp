using System.Collections.Generic;
using XMLSchemaDefinition;
using static ColladaSharp.Definition.Collada;

namespace ColladaSharp.Definition
{
    public interface ITechnique : IElement { }
    public interface IInputShared : IElement { }
    public interface IInputUnshared : IElement { }
    /// <summary>
    /// Indicates this class is an owner of an Asset element.
    /// </summary>
    public interface IAsset : IElement
    {
        Asset AssetElement { get; }
    }
    /// <summary>
    /// Indicates this class is an owner of a DataFlowParam element.
    /// </summary>
    public interface IDataFlowParam : IElement { }
    /// <summary>
    /// Indicates this class is an owner of a RefParam element.
    /// </summary>
    public interface IRefParam : IElement { }
    /// <summary>
    /// Indicates this class is an owner of a SetParam element.
    /// </summary>
    public interface ISetParam : IElement { }
    /// <summary>
    /// Indicates this class is an owner of a NewParam element.
    /// </summary>
    public interface INewParam : IElement { }
    /// <summary>
    /// Indicates this class is an owner of an Annotate element.
    /// </summary>
    public interface IAnnotate : IElement { }
    public interface IInstanceMesh : IElement
    {
        BindMaterial BindMaterialElement { get; }
    }
    public interface IInstantiatable : IElement { }
    public interface IInstanceElement : IElement { }
    public interface IArrayElement : IElement { }
    public interface ISource : IElement { }
    public interface ISIDAncestor : IElement
    {
        List<ISID> SIDElementChildren { get; }
    }
    public interface IID : ISIDAncestor
    {
        string ID { get; set; }
    }
    public interface ISID : ISIDAncestor
    {
        string SID { get; set; }
    }
    public interface IElementName { string Name { get; set; } }

    public interface IExtra : IElement
    {
        Extra[] ExtraElements { get; }
    }
}

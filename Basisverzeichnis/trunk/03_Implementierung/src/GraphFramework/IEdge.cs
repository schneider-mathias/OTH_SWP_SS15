namespace GraphFramework
{
    public interface IEdge : IGraphElement
    {
        /// <summary>
        /// the first node to which the edge is connected
        /// </summary>
        INode Node1 { get; set; }

        /// <summary>
        /// the second node to which the edge is connected
        /// </summary>
        INode Node2 { get; set; }
    }
}
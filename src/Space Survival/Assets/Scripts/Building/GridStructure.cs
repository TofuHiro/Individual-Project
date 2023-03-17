public class GridStructure
{
    public bool Floor;
    public bool FrontWall;
    public bool RightWall;
    public bool BackWall;
    public bool LeftWall;

    public bool IsSealed;

    public bool IsEmpty { get { return !Floor && !FrontWall && !RightWall && !BackWall && !LeftWall; } }

    /// <summary>
    /// Get the reference of the corresponding edge bool
    /// </summary>
    /// <param name="_edge">The edge to get</param>
    /// <returns>The bool ref of the given edge</returns>
    public ref bool GetEdge(Edge _edge)
    {
        switch (_edge) {
            case Edge.Right:
                return ref RightWall;
            case Edge.Back:
                return ref BackWall;
            case Edge.Left:
                return ref LeftWall;
            default:
                //Edge.Front
                return ref FrontWall;
        }
    }
}

using XNode;

namespace InteractML
{
    /// <summary>
    /// Structure of a pair of input/ouput ports for an IMLNode
    /// </summary>
    public class IMLNodePortPair
    {
        public NodePort input;
        public NodePort output;

        public IMLNodePortPair()
        {
            input = null;
            output = null;
        }

        public IMLNodePortPair(NodePort input, NodePort output)
        {
            this.input = input;
            this.output = output;
        }

        public void Reset()
        {
            input = null;
            output = null;
        }
    }

}

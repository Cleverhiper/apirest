using System.Collections.Generic;

namespace apirest.HATEOAS
{
    public class HATEOAS
    {
        private string url;
        private string protocol = "https://";
        public List<Link> actions = new List<Link>();
        public HATEOAS(string url) {
            this.url = url;
        }

        public HATEOAS(string url, string protocol)
        {
            this.url = url;
            this.protocol = protocol;
        }

        public void AddAction(string rel, string method)
        {
            actions.Add(new Link(this.protocol + this.url,rel,method));
        }

        public Link[] GetActions()
        {
            return actions.ToArray();
            
        }
    }
}
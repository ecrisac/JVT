namespace TimeReport
{
    public class JiraItem
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string AuthorName { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
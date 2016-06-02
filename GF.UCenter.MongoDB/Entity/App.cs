namespace GF.UCenter.MongoDB.Entity
{
    public class App : EntityBase
    {
        public string Name { get; set; }
        public string AppSecret { get; set; }
        public string Configuration { get; set; }
    }
}
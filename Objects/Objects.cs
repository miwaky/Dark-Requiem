namespace DarkRequiem.objects
{
    public class Objects
    {
        public int id { get; private set; }
        public string name { get; private set; }
        public string type { get; private set; }
        public int spriteId { get; private set; }
        public int quantity { get; set; }

        public Objects(int pId, string pName, string pType, int pSpriteId, int pQuantity)
        {
            id = pId;
            name = pName;
            type = pType;
            spriteId = pSpriteId;
            quantity = pQuantity;
        }
    }
}

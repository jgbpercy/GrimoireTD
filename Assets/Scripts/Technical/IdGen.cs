namespace GrimoireTD.Technical
{
    public static class IdGen
    {
        private static int nextId = 0;

        public static int GetNextId()
        {
            nextId++;
            return nextId;
        }
    }
}
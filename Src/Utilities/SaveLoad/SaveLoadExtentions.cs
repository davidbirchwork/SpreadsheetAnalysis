namespace Utilities.SaveLoad {
    public static class SaveLoadExtentions {

        public static bool Save(this ISaveLoad item) {
            return SaveLoad.Save(item);
        }
        
    }
}

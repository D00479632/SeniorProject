using StardewValley;
using StardewValley.ItemTypeDefinitions;

public class MenuData
{
    public string HeaderText { get; set; } = "";
    public List<ParsedItemData> Items { get; set; } = new();
    public static MenuData Edibles()
    {
        int[] edibleCategories = new int[]
        {
            StardewValley.Object.CookingCategory,
            StardewValley.Object.EggCategory,
            StardewValley.Object.FishCategory,
            StardewValley.Object.FruitsCategory,
            StardewValley.Object.meatCategory,
            StardewValley.Object.MilkCategory,
            StardewValley.Object.VegetableCategory
        };
        var items = ItemRegistry.ItemTypes
            .Single(type => type.Identifier == ItemRegistry.type_object)
            .GetAllIds()
            .Select(id => ItemRegistry.GetDataOrErrorItem(id))
            .Where(data => edibleCategories.Contains(data.Category))
            .ToList();
        return new()
        {
            HeaderText = "All Edibles",
            Items = items,
        };
    }
}
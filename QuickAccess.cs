using BTKUILib.UIObjects;

namespace BTKUILib;

public class QuickAccess
{
    internal static QuickAccess Instance;

    internal Page QuickAccessPage;
    internal Category MainCategory;

    internal static void SetupQuickAccess()
    {
        if (Instance != null) return;

        Instance = new QuickAccess();
        Instance.SetupQuickAccessInstance();
    }

    private void SetupQuickAccessInstance()
    {
        QuickAccessPage = new Page("btkUI-QuickAccessPage");
        QuickAccessPage.PageDisplayName = "Quick Access";

        MainCategory = QuickAccessPage.AddCategory("Nope", false);


    }
}

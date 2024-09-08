using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using System.Collections.Generic;

namespace BTKUILib;

internal class PlayerList
{
    internal static PlayerList Instance;

    internal Page PlayerSelectPage;
    internal CVRPlayerEntity SelectedPlayer;

    private Page _internalPlayerListPage;
    private Category _internalPlayerListCategory;
    private readonly Dictionary<string, Button> _userButtons = new();

    //PlayerSelectPage internal objects
    private Category _internalSelectCategory;
    private ToggleButton _muteUser;
    private SliderFloat _playerVolume;
    private MultiSelection _avatarBlockMode, _propBlockMode;
    private CVRSelfModerationEntryUi _moderationEntry;

    internal static void SetupPlayerList()
    {
        if (Instance != null) return;

        Instance = new PlayerList();
        Instance.SetupPlayerListInstance();
    }

    internal void OpenPlayerActionPage(CVRPlayerEntity player)
    {
        //Update the player settings page data
        SelectedPlayer = player;
        QuickMenuAPI.SelectedPlayerName = player.Username;
        QuickMenuAPI.SelectedPlayerID = player.Uuid;
        QuickMenuAPI.OnPlayerSelected?.Invoke(player.Username, player.Uuid);
        QuickMenuAPI.OnPlayerEntitySelected?.Invoke(player);

        _moderationEntry = MetaPort.Instance.SelfModerationManager.GetPlayerSelfModerationProfile(player.Uuid, player.AvatarId);
        _muteUser.ToggleValue = _moderationEntry.mute;
        _playerVolume.SetSliderValue(_moderationEntry.voiceVolume*100f);
        _propBlockMode.SetSelectedOptionWithoutAction(_moderationEntry.userPropVisibility);
        _avatarBlockMode.SetSelectedOptionWithoutAction(_moderationEntry.userAvatarVisibility);

        QuickMenuAPI.PlayerSelectPage.PageDisplayName = player.Username;
        QuickMenuAPI.PlayerSelectPage.OpenPage();
    }

    private void SetupPlayerListInstance()
    {
        //Attach to events
        QuickMenuAPI.UserJoin += UserJoin;
        QuickMenuAPI.UserLeave += UserLeave;
        QuickMenuAPI.OnWorldLeave += OnWorldLeave;
        QuickMenuAPI.OnMenuGenerated += OnMenuGenerated;

        //Attach to existing page within JS
        _internalPlayerListPage = new Page("btkUI-PlayerList");
        _internalPlayerListPage.PageDisplayName = "Playerlist | 0 Players in World";
        _internalPlayerListCategory = _internalPlayerListPage.AddCategory("Players", false, false);

        //Setup playerlist action page
        PlayerSelectPage = new Page("btkUI-PlayerSelectPage");
        _internalSelectCategory = PlayerSelectPage.AddCategory("Quick Settings", "BTKUILib");
        _muteUser = _internalSelectCategory.AddToggle("Mute User", "Mutes the selected user", false);
        _muteUser.OnValueUpdated += MuteUser;
        _avatarBlockMode = new MultiSelection("Avatar Visibility", ["Hidden", "Use Content Filter", "Shown"], 1);
        _avatarBlockMode.OnOptionUpdated += AvatarBlockModeUpdate;
        _propBlockMode = new MultiSelection("Prop Visibility", ["Hidden", "Use Content Filter", "Shown"], 1);
        _propBlockMode.OnOptionUpdated += PropBlockModeUpdate;
        var avatarBlockSetting = _internalSelectCategory.AddButton("Avatar Visibility", "Visibility", "Sets this users avatar visibility");
        avatarBlockSetting.OnPress += OpenAvatarBlockSetting;
        var propBlockSetting = _internalSelectCategory.AddButton("Prop Visibility", "Visibility", "Sets this users prop visibility");
        propBlockSetting.OnPress += OpenPropBlockSetting;
        var reloadAvatar = _internalSelectCategory.AddButton("Reload Avatar", "Reload", "Reloads this users avatar");
        reloadAvatar.OnPress += ReloadAvatar;
        var blockUser = _internalSelectCategory.AddButton("Block User", "BlockUser", "Blocks the selected user");
        blockUser.OnPress += BlockUser;
        _playerVolume = _internalSelectCategory.AddSlider("Player Voice Volume", "Adjust this players voice volume", 100, 0, 200);
        _playerVolume.OnValueUpdated += AdjustVoiceVolume;
    }

    private void MuteUser(bool state)
    {
        MetaPort.Instance.SelfModerationManager.SetPlayerMute(SelectedPlayer.Uuid, state);
    }

    private void PropBlockModeUpdate(int state)
    {
        switch (state)
        {
            case 0:
                //Hidden
                MetaPort.Instance.SelfModerationManager.SetPlayerPropVisibility(SelectedPlayer.Uuid, false);
                break;
            case 1:
                //Reset
                MetaPort.Instance.SelfModerationManager.ResetPlayerPropVisibility(SelectedPlayer.Uuid);
                break;
            case 2:
                //Shown
                MetaPort.Instance.SelfModerationManager.SetPlayerPropVisibility(SelectedPlayer.Uuid, true);
                break;
        }
    }

    private void AvatarBlockModeUpdate(int state)
    {
        switch (state)
        {
            case 0:
                //Hidden
                MetaPort.Instance.SelfModerationManager.SetPlayerAvatarVisibility(SelectedPlayer.Uuid, false);
                break;
            case 1:
                //Reset
                MetaPort.Instance.SelfModerationManager.ResetPlayerAvatarVisibility(SelectedPlayer.Uuid);
                break;
            case 2:
                //Shown
                MetaPort.Instance.SelfModerationManager.SetPlayerAvatarVisibility(SelectedPlayer.Uuid, true);
                break;
        }
    }

    private void AdjustVoiceVolume(float level)
    {
        MetaPort.Instance.SelfModerationManager.SetPlayerVolume(SelectedPlayer.Uuid, level/100f);
    }

    private void BlockUser()
    {
        QuickMenuAPI.ShowConfirm("Block User?", $"Are you sure you want to block {SelectedPlayer.Username}? You'll need to go to the big menu to undo this!", () =>
        {
            ViewManager.Instance.BlockUser(SelectedPlayer.Uuid);
        });
    }

    private void ReloadAvatar()
    {
        CVRPlayerManager.Instance.ReloadPlayersAvatar(SelectedPlayer.Uuid);
    }

    private void OpenPropBlockSetting()
    {
        QuickMenuAPI.OpenMultiSelect(_propBlockMode);
    }

    private void OpenAvatarBlockSetting()
    {
        QuickMenuAPI.OpenMultiSelect(_avatarBlockMode);
    }

    private void OnMenuGenerated(CVR_MenuManager _)
    {
        _internalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
        //Force page to IsVisible
        _internalPlayerListPage.IsVisible = true;
        _internalPlayerListPage.GenerateCohtml();
    }

    private void OnWorldLeave()
    {
        _internalPlayerListCategory.ClearChildren();
        _userButtons.Clear();
        _internalPlayerListPage.PageDisplayName = "Playerlist | 0 Players in World";
    }

    private void UserLeave(CVRPlayerEntity player)
    {
        if (!_userButtons.TryGetValue(player.Uuid, out var button)) return;

        button.Delete();
        _userButtons.Remove(player.Uuid);

        _internalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
    }

    private void UserJoin(CVRPlayerEntity player)
    {
        if (_userButtons.ContainsKey(player.Uuid)) return;

        var newUserBtn = _internalPlayerListCategory.AddButton(player.Username, player.ApiProfileImageUrl, $"Opens the player options for {player.Username}!", ButtonStyle.FullSizeImage);
        newUserBtn.OnPress += () =>
        {
            //User select
            OpenPlayerActionPage(player);
        };

        _userButtons.Add(player.Uuid, newUserBtn);

        _internalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
    }
}

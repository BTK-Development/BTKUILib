using ABI_RC.Core.InteractionSystem;
using ABI_RC.Core.Networking.API;
using ABI_RC.Core.Networking.API.UserWebsocket;
using ABI_RC.Core.Networking.IO.Social;
using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using BTKUILib.UIObjects;
using BTKUILib.UIObjects.Components;
using BTKUILib.UIObjects.Objects;
using System;
using System.Collections.Generic;

namespace BTKUILib;

internal class PlayerList
{
    internal static PlayerList Instance;

    internal Page PlayerSelectPage;
    internal UIPlayerObject SelectedPlayer;
    internal Page InternalPlayerListPage;

    private Category _internalPlayerListCategory;
    private readonly Dictionary<string, Button> _userButtons = new();

    //PlayerSelectPage internal objects
    private Category _internalSelectCategory;
    private ToggleButton _muteUser;
    private SliderFloat _playerVolume;
    private MultiSelection _avatarBlockMode, _propBlockMode;
    private CVRSelfModerationEntryUi _moderationEntry;
    private bool _playerSelectMode;
    private Action<UIPlayerObject> _playerSelectCallback;
    private UIPlayerObject _localUserObject;
    private Button _friendButton;

    internal static void SetupPlayerList()
    {
        if (Instance != null) return;

        Instance = new PlayerList();
        Instance.SetupPlayerListInstance();
    }

    internal void OpenPlayerActionPage(UIPlayerObject player)
    {
        if (_playerSelectMode)
        {
            //A player was selected, fire the callback
            _playerSelectCallback?.Invoke(player);

            _playerSelectMode = false;
            _playerSelectCallback = null;

            QuickMenuAPI.GoBack();
            ResetPlayerlist();
            return;
        }

        //Update the player settings page data
        SelectedPlayer = player;
        QuickMenuAPI.SelectedPlayerName = player.Username;
        QuickMenuAPI.SelectedPlayerID = player.Uuid;
        QuickMenuAPI.OnPlayerSelected?.Invoke(player.Username, player.Uuid);
        QuickMenuAPI.OnPlayerEntitySelected?.Invoke(player);

        _internalSelectCategory.Hidden = player.IsLocalUser;

        _moderationEntry = MetaPort.Instance.SelfModerationManager.GetPlayerSelfModerationProfile(player.Uuid, player.AvatarID);
        _muteUser.ToggleValue = _moderationEntry.mute;
        _playerVolume.SetSliderValue(_moderationEntry.voiceVolume*100f);
        _propBlockMode.SetSelectedOptionWithoutAction(_moderationEntry.userPropVisibility);
        _avatarBlockMode.SetSelectedOptionWithoutAction(_moderationEntry.userAvatarVisibility);
        _friendButton.ButtonText = Friends.FriendsWith(player.Uuid) ? "Unfriend" : "Add Friend";
        _friendButton.ButtonTooltip = Friends.FriendsWith(player.Uuid) ? "Unfriend this user" : "Send a friend request to this user";
        _friendButton.ButtonIcon = Friends.FriendsWith(player.Uuid) ? "UserMinus" : "UserAdd";

        QuickMenuAPI.PlayerSelectPage.PageDisplayName = player.Username;
        QuickMenuAPI.PlayerSelectPage.OpenPage();
    }

    internal void OpenPlayerPicker(string title, Action<UIPlayerObject> callback)
    {
        InternalPlayerListPage.PageDisplayName = title;
        foreach (var button in _userButtons.Values)
            button.ButtonTooltip = $"Selects {button.ButtonText}";

        _playerSelectMode = true;
        _playerSelectCallback = callback;

        InternalPlayerListPage.OpenPage();
    }

    internal void ResetListAfterConnRecovery()
    {
        _internalPlayerListCategory.ClearChildren();
        _userButtons.Clear();

        AddLocalUser();

        foreach (var player in CVRPlayerManager.Instance.NetworkPlayers)
        {
            UserJoin(player);
        }
    }

    internal void OnWorldJoin()
    {
        AddLocalUser();
    }

    private void SetupPlayerListInstance()
    {
        //Attach to events
        QuickMenuAPI.UserJoin += UserJoin;
        QuickMenuAPI.UserLeave += UserLeave;
        QuickMenuAPI.OnWorldLeave += OnWorldLeave;
        QuickMenuAPI.OnMenuGenerated += OnMenuGenerated;

        //Attach to existing page within JS
        InternalPlayerListPage = new Page("btkUI-PlayerList");
        InternalPlayerListPage.PageDisplayName = "Playerlist | 0 Players in World";
        _internalPlayerListCategory = InternalPlayerListPage.AddCategory("Players", false, false);

        InternalPlayerListPage.OnPageClosed += OnPageClosed;

        _localUserObject = new UIPlayerObject(null);

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
        _friendButton = _internalSelectCategory.AddButton("Add Friend", "UserAdd", "Sends a friend request to this user!");
        _friendButton.OnPress += FriendButtonPress;
        var kickUser = _internalSelectCategory.AddButton("Kick User", "ExitDoor", "Kick this user from the instance (Only instance owner/moderator)");
        kickUser.OnPress += KickUser;
        var voteKick = _internalSelectCategory.AddButton("Vote Kick", "ThumbsDown", "Start a vote kick against this user!");
        voteKick.OnPress += VoteKick;
        _playerVolume = _internalSelectCategory.AddSlider("Player Voice Volume", "Adjust this players voice volume", 100, 0, 200);
        _playerVolume.OnValueUpdated += AdjustVoiceVolume;
    }

    private void VoteKick()
    {
        QuickMenuAPI.ShowConfirm("Start Vote Kick?", $"Are you sure you want to start a vote kick against {SelectedPlayer.Username}? They won't be able to rejoin for an hour!", () =>
        {
            ViewManager.Instance.StartVoteKick(SelectedPlayer.Uuid);
        });
    }

    private void KickUser()
    {
        QuickMenuAPI.ShowConfirm("Kick User?", $"Are you sure you want to kick from the instance {SelectedPlayer.Username}? They won't be able to rejoin for an hour!", () =>
        {
            UIUtils.KickUser(SelectedPlayer.Uuid);
        });
    }

    private void FriendButtonPress()
    {
        if (Friends.FriendsWith(SelectedPlayer.Uuid))
        {
            //Remove friend
            QuickMenuAPI.ShowConfirm("Remove Friend?", $"Are you sure you want to unfriend {SelectedPlayer.Username}?", () =>
            {
                ApiConnection.SendWebSocketRequest(RequestType.UnFriend, new
                {
                    id = SelectedPlayer.Uuid
                });
            });
        }
        else
        {
            QuickMenuAPI.ShowConfirm("Send Friend Request?", $"Are you sure you want to send a friend request to {SelectedPlayer.Username}?", () =>
            {
                ApiConnection.SendWebSocketRequest(RequestType.FriendRequestSend, new
                {
                    id = SelectedPlayer.Uuid
                });
            });
        }
    }

    private void OnPageClosed()
    {
        if (!_playerSelectMode) return;

        ResetPlayerlist();
    }

    private void ResetPlayerlist()
    {
        InternalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
        foreach (var button in _userButtons.Values) button.ButtonTooltip = $"Opens the player options for {button.ButtonText}";
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
        if(!_playerSelectMode)
            InternalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
        //Force page to IsVisible
        InternalPlayerListPage.IsVisible = true;
        InternalPlayerListPage.GenerateCohtml();
    }

    private void OnWorldLeave()
    {
        _internalPlayerListCategory.ClearChildren();
        _userButtons.Clear();
        if(!_playerSelectMode)
            InternalPlayerListPage.PageDisplayName = "Playerlist | 0 Players in World";

        AddLocalUser();
    }

    private void UserLeave(CVRPlayerEntity player)
    {
        if (!_userButtons.TryGetValue(player.Uuid, out var button)) return;

        button.Delete();
        _userButtons.Remove(player.Uuid);

        if(!_playerSelectMode)
            InternalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
    }

    private void UserJoin(CVRPlayerEntity player)
    {
        if(_userButtons.Count == 0)
            AddLocalUser();

        if (_userButtons.ContainsKey(player.Uuid)) return;

        var playerObject = new UIPlayerObject(player);

        var newUserBtn = _internalPlayerListCategory.AddButton(player.Username, player.ApiProfileImageUrl, $"Opens the player options for {player.Username}!", ButtonStyle.FullSizeImage);
        newUserBtn.OnPress += () =>
        {
            //User select
            OpenPlayerActionPage(playerObject);
        };

        _userButtons.Add(player.Uuid, newUserBtn);

        if(!_playerSelectMode)
            InternalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
    }

    private void AddLocalUser()
    {
        if (_userButtons.ContainsKey(_localUserObject.Uuid)) return;

        var newUserBtn = _internalPlayerListCategory.AddButton(_localUserObject.Username, _localUserObject.PlayerIconURL, $"Opens the player options for {_localUserObject.Username}!", ButtonStyle.FullSizeImage);
        newUserBtn.OnPress += () =>
        {
            //User select
            OpenPlayerActionPage(_localUserObject);
        };

        _userButtons.Add(_localUserObject.Uuid, newUserBtn);

        if(!_playerSelectMode)
            InternalPlayerListPage.PageDisplayName = $"Playerlist | {_userButtons.Count} Players in World";
    }
}

using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using UnityEngine;

namespace BTKUILib;

/// <summary>
/// Wrapper object for CVRPlayerEntity, used to make handling local user information a bit easier
/// </summary>
public class UIPlayerObject
{
    internal ABI_RC.Systems.UI.UILib.UIPlayerObject InternalPlayerObject;

    public UIPlayerObject(ABI_RC.Systems.UI.UILib.UIPlayerObject internalPlayerObject)
    {
        InternalPlayerObject = internalPlayerObject;
    }

    /// <summary>
    /// Returns full CVRPlayerEntity for remote users, null for local
    /// </summary>
    public CVRPlayerEntity CVRPlayer => InternalPlayerObject.CVRPlayer;

    /// <summary>
    /// returns the avatar object
    /// </summary>
    public GameObject AvatarObject => InternalPlayerObject.AvatarObject;

    /// <summary>
    /// Returns the UUID for this user
    /// </summary>
    public string Uuid => InternalPlayerObject.Uuid;

    /// <summary>
    /// Returns the Username of this user
    /// </summary>
    public string Username => InternalPlayerObject.Username;
    /// <summary>
    /// Returns the private animator from the PuppetMaster
    /// </summary>
    public Animator AvatarAnimator => InternalPlayerObject.AvatarAnimator;

    /// <summary>
    /// Returns the player's root gameobject
    /// </summary>
    public GameObject PlayerGameObject => InternalPlayerObject.PlayerGameObject;

    /// <summary>
    /// Returns the AvatarID of this user
    /// </summary>
    public string AvatarID => InternalPlayerObject.AvatarID;

    /// <summary>
    /// Returns the player ImageURL from the API, if local user is null API didn't give us the user details
    /// </summary>
    public string PlayerIconURL => InternalPlayerObject.PlayerIconURL;

    /// <summary>
    /// Returns true if this UIPlayerObject is the local user
    /// </summary>
    public bool IsLocalUser => InternalPlayerObject.IsLocalUser;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"UIPlayerObject - [Uuid: {Uuid}, Username: {Username}]";
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if(obj is UIPlayerObject playerObject)
            return Uuid == playerObject.Uuid;
        return false;
    }
}
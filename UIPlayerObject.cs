using ABI_RC.Core.Player;
using ABI_RC.Core.Savior;
using UnityEngine;

namespace BTKUILib;

/// <summary>
/// Wrapper object for CVRPlayerEntity, used to make handling local user information a bit easier
/// </summary>
public class UIPlayerObject
{
        private readonly CVRPlayerEntity _playerEntity;
        private readonly bool _isRemotePlayer;

        internal UIPlayerObject(CVRPlayerEntity playerEntity)
        {
            _playerEntity = playerEntity;

            if (ReferenceEquals(playerEntity, null)) return;

            _isRemotePlayer = true;
        }

        /// <summary>
        /// Returns full CVRPlayerEntity for remote users, null for local
        /// </summary>
        public CVRPlayerEntity CVRPlayer => _isRemotePlayer ? _playerEntity : null;

        /// <summary>
        /// returns the avatar object
        /// </summary>
        public GameObject AvatarObject
        {
            get
            {
                if (!_isRemotePlayer)
                    return PlayerSetup.Instance._avatar;

                return _playerEntity.PuppetMaster == null ? null : _playerEntity.PuppetMaster.avatarObject;
            }
        }

        /// <summary>
        /// Returns the UUID for this user
        /// </summary>
        public string Uuid
        {
            get
            {
                if (!_isRemotePlayer)
                    return MetaPort.Instance.ownerId;
                return ReferenceEquals(_playerEntity, null) ? null : _playerEntity.Uuid;
            }
        }

        /// <summary>
        /// Returns the Username of this user
        /// </summary>
        public string Username
        {
            get
            {
                if (!_isRemotePlayer)
                    return UIUtils.GetSelfUsername();
                return ReferenceEquals(_playerEntity, null) ? null : _playerEntity.Username;
            }
        }

        /// <summary>
        /// Returns the private animator from the PuppetMaster
        /// </summary>
        public Animator AvatarAnimator
        {
            get
            {
                if (!_isRemotePlayer)
                    return PlayerSetup.Instance._animator;
                return ReferenceEquals(_playerEntity, null) ? null : UIUtils.GetAvatarAnimator(_playerEntity.PuppetMaster);
            }
        }

        /// <summary>
        /// Returns the player's root gameobject
        /// </summary>
        public GameObject PlayerGameObject
        {
            get
            {
                if (!_isRemotePlayer)
                    return PlayerSetup.Instance.gameObject;
                return ReferenceEquals(_playerEntity, null) ? null : _playerEntity.PlayerObject;
            }
        }

        /// <summary>
        /// Returns the player position, remote users require some weirdness
        /// </summary>
        public Vector3 PlayerPosition
        {
            get
            {
                if (!_isRemotePlayer)
                    return PlayerSetup.Instance.GetPlayerPosition();
                // remote players avatar root is stuck at their playspace center, game bug :)
                return ReferenceEquals(_playerEntity, null)
                    ? Vector3.zero
                    : _playerEntity.PuppetMaster.GetViewWorldPosition() with
                    {
                        y = _playerEntity.PuppetMaster.transform.position.y
                    };
            }
        }

        /// <summary>
        /// Returns the AvatarID of this user
        /// </summary>
        public string AvatarID
        {
            get
            {
                if (!_isRemotePlayer)
                    return MetaPort.Instance.currentAvatarGuid;
                return ReferenceEquals(_playerEntity, null) ? null : _playerEntity.AvatarId;
            }
        }

        /// <summary>
        /// Returns the player ImageURL from the API, if local user is null API didn't give us the user details
        /// </summary>
        public string PlayerIconURL
        {
            get
            {
                if (!_isRemotePlayer)
                    return Patches.LocalUserDetails?.ImageUrl;
                return ReferenceEquals(_playerEntity, null) ? "" : _playerEntity.ApiProfileImageUrl;
            }
        }

        /// <summary>
        /// Returns true if this UIPlayerObject is the local user
        /// </summary>
        public bool IsLocalUser => !_isRemotePlayer;

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

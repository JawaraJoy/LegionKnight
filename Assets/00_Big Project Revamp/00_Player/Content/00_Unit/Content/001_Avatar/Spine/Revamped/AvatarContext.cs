using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class AvatarContext
    {
        [SerializeField, MMReadOnly]
        private Unit m_OwnerObject;
        [SerializeField, MMReadOnly]
        private AvatarSpine m_AvatarSpine;

        public Unit OwnerObject => m_OwnerObject;
        public AvatarSpine AvatarSpine => m_AvatarSpine;
        public bool Initialized => m_OwnerObject != null && m_AvatarSpine != null;
        public AvatarContext(Unit ownerObject, AvatarSpine avatarSpine) 
        { 
            m_OwnerObject = ownerObject;
            m_AvatarSpine = avatarSpine;
        }

    }
}

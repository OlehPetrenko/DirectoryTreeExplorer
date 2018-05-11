using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace DirectoryTreeExplorer.Business
{
    /// <summary>
    /// Represents a logic to get owner of a system resource. 
    /// </summary>
    public sealed class OwnerProvider
    {
        private const string NotAvailableOwnerText = "Not available";

        public string GetOwner(string path)
        {
            IdentityReference sid = null;

            try
            {
                sid = File.GetAccessControl(path, AccessControlSections.Owner).GetOwner(typeof(SecurityIdentifier));

                GlobalCache.Instance.Owners.TryGetValue(sid.Value, out var owner);
                if (!string.IsNullOrEmpty(owner))
                    return owner;

                var ntAccount = sid.Translate(typeof(NTAccount));

                GlobalCache.Instance.Owners.Add(sid.Value, ntAccount.ToString());

                return ntAccount.ToString();
            }
            catch (Exception)
            {
                if (sid != null)
                    GlobalCache.Instance.Owners.Add(sid.Value, NotAvailableOwnerText);

                return NotAvailableOwnerText;
            }
        }
    }
}

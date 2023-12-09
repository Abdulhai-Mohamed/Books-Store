using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Metrics;

namespace Books_Store.Security.EncryptionAndDecryption
{
    public class OurCustomDataProtectionPurposeStrings
    {

        //You can think of purpose string as an encryption key.This key is then combined with the master or
        //root key to generate a unique key.The data that is encrypted by a given combination of purpose string
        //and root key can only be decrypted by that same combination of keys.
        //The purpose string is inherent to the security of the data protection system, as it provides
        //isolation between cryptographic consumers, even if the root keys are the same.


        //this is the property that will used as the propose when  create the protector
        public readonly string AuthorIdRouteValue = "AuthorIdRouteValue";
    }
}

﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Books_Store.Security.CustomDataProtectorTokenProvider
{
    public class CustomEmailConfirmationTokenProvider<TUser>
    : DataProtectorTokenProvider<TUser> where TUser : class
    {

        public CustomEmailConfirmationTokenProvider
        (
                    IDataProtectionProvider dataProtectionProvider,
                    IOptions<CustomEmailConfirmationTokenProviderOptions> options,
                    ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger
                    
        ) : base(dataProtectionProvider, options, logger)
        {


        }


    }
}
﻿using SysTk.WebApi.Data.Models;

namespace SysTk.WebAPI.GraphQL.Types
{
    public class FtpCredentialType : ObjectType<FtpCredentials>
    {
        protected override void Configure(IObjectTypeDescriptor<FtpCredentials> descriptor)
        {
            descriptor.Description("Represents a set of FTP credentials (username and password) for a FuelPOS station");

            descriptor.Field(x => x.Id)
                .Type<NonNullType<IdType>>();

            descriptor.Field(x => x.StationId)
                .Type<NonNullType<IdType>>();
        }
    }
}

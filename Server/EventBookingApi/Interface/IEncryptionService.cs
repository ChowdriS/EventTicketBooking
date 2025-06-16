using System;
using EventBookingApi.Model;

namespace EventBookingApi.Interface;

public interface IEncryptionService
    {
        public Task<EncryptModel> EncryptData(EncryptModel data);
    }
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

public class MyCertificateHandler : CertificateHandler
{
    private static string PUB_KEY =
        "-----BEGIN PUBLIC KEY-----" +
        "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAqZz7YZpj/+ltaw+JOpPe" +
        "UHHq1j16TPqnAMT07CGV8WpRtoBFSrWGypgJ6kGaFx+cfn2zVc8yCuwyT0ysVzRx" +
        "A3e85rhQbkm7JzPrZz6uXNbN2kqF2bavX7Tz4Pk058piLZQ/IZ0a5gQHwwVU5XSE" +
        "huXmeEDoaVCO6c0tG4XrS3eSh8Gi3TMuPslGzVs3xQqv/BfVCSQoPjvOCHN/zMZH" +
        "OhCDwUBbCNO6DmE3ON5YHx2yhnm7pg6p0XITV86mxeEZfHKzGM9m8/8OMpZyqA/l" +
        "6FcWoPlJF0CDDcUyd0sYrbfgsiXRx3QCLG+ulb8GeqGgFPMFfn1VwpTNW1F4zjF+" +
        "4wIDAQAB" +
        "-----END PUBLIC KEY-----";

    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
        /*
        X509Certificate2 certificate = new X509Certificate2(certificateData);
        string pk = certificate.GetPublicKeyString();
        if (pk.Equals(PUB_KEY))
            return true;

        // Bad dog
        return false;
        */
    }

}

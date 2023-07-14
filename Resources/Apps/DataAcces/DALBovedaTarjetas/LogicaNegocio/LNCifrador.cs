using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace DALBovedaTarjetas.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para el cifrado de objetos
    /// </summary>
    public class LNCifrador
    {
        public static PgpPublicKeyRing asciiPublicKeyToRing(string ascfilein, ILogHeader logHeader)
        {
            var pkr = null as PgpPublicKeyRing;

            try
            {
                using (Stream pubFis = File.OpenRead(ascfilein))
                {

                    var pubArmoredStream = new ArmoredInputStream(pubFis);

                    PgpObjectFactory pgpFact = new PgpObjectFactory(pubArmoredStream);
                    Object opgp = pgpFact.NextPgpObject();
                    pkr = opgp as PgpPublicKeyRing;
                }

            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Error en el cifrado del archivo (PK)");
            }

            return pkr;
        }

        public static PgpPublicKey getFirstPublicEncryptionKeyFromRing(PgpPublicKeyRing pkr)
        {
            foreach (PgpPublicKey k in pkr.GetPublicKeys())
            {
                if (k.IsEncryptionKey)
                    return k;
            }

            throw new CAppException(8011, "Error en el cifrado del archivo (EK)");
        }

        public static void EncryptFile(string inputFile, string outputFile, PgpPublicKey encKey, bool armor,
            bool withIntegrityCheck, ILogHeader logHeader)
        {
            try
            {
                using (MemoryStream bOut = new MemoryStream())
                {
                    PgpCompressedDataGenerator comData = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);
                    PgpUtilities.WriteFileToLiteralData(comData.Open(bOut), PgpLiteralData.Binary,
                        new FileInfo(inputFile));

                    comData.Close();
                    PgpEncryptedDataGenerator cPk = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Aes256,
                        withIntegrityCheck, new SecureRandom());

                    cPk.AddMethod(encKey);
                    byte[] bytes = bOut.ToArray();

                    using (Stream outputStream = File.Create(outputFile))
                    {
                        if (armor)
                        {
                            using (ArmoredOutputStream armoredStream = new ArmoredOutputStream(outputStream))
                            using (Stream cOut = cPk.Open(armoredStream, bytes.Length))
                            {
                                cOut.Write(bytes, 0, bytes.Length);
                            }
                        }
                        else
                        {
                            using (Stream cOut = cPk.Open(outputStream, bytes.Length))
                            {
                                cOut.Write(bytes, 0, bytes.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogPCI log = new LogPCI(logHeader);
                log.ErrorException(ex);
                throw new CAppException(8011, "Error al cifrar archivo");
            }
        }
    }
}

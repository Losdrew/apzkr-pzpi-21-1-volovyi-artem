export interface CertificateInfoDto {
  subject: string,
  issuer: string,
  issuedDate: Date,
  expiryDate: Date,
  thumbprint: string,
}
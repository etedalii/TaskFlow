export interface AuthResponse {
    token: string,
    expiresAt: Date,
    refreshToken: string
}
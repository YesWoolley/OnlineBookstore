export interface User {
    // Core identification (from UserDto)
    id: string;
    userName: string;
    email: string;
    firstName?: string;
    lastName?: string;
    phoneNumber?: string;
    createdAt: string; // DateTime from backend converted to string

    // Roles from backend
    roles: string[];

    // Computed property from backend
    fullName?: string;
}

// For authentication forms (matching LoginDto)
export interface UserCredentials {
    userName: string;
    password: string;
    rememberMe?: boolean;
}

// For registration forms (matching RegisterDto)
export interface UserRegistration {
    userName: string;
    email: string;
    password: string;
    confirmPassword: string;
    firstName?: string;
    lastName?: string;
    phoneNumber?: string;
}

// For profile updates (matching UpdateProfileDto)
export interface UserProfileUpdate {
    firstName?: string;
    lastName?: string;
    phoneNumber?: string;
    email: string;  
}

// For password changes (matching ChangePasswordDto)
export interface PasswordChange {
    currentPassword: string;
    newPassword: string;
    confirmNewPassword: string;
}

// For API responses (matching AuthResultDto)
export interface AuthResponse {
    success: boolean;
    token?: string;
    refreshToken?: string;
    user?: User;
    message?: string;
}

// For password reset requests
export interface PasswordResetRequest {
    email: string;
}

export interface PasswordReset {
    token: string;
    newPassword: string;
    confirmPassword: string;
}

// Extended user interface for internal use (includes backend-only fields)
export interface ExtendedUser extends User {
    // Additional fields from ApplicationUser (not exposed in UserDto)
    refreshToken?: string;
    refreshTokenExpiryTime?: string;

    // Navigation properties (for internal use)
    orders?: any[];
    reviews?: any[];
    shoppingCartItems?: any[];
}

// User summary for lists and cards
export interface UserSummary {
    id: string;
    userName: string;
    fullName?: string;
    email: string;
    roles: string[];
}
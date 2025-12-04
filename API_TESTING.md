# API Testing Guide

## Prerequisites
1. Start infrastructure: `docker-compose up -d`
2. Run API: `cd src\ModernBaseProject.API && dotnet run`
3. API should be running on http://localhost:5000

## Step 1: Login

```powershell
$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{"email":"admin@domain.com","password":"Admin123!"}'

$token = $loginResponse.accessToken
Write-Host "Token: $token"
```

## Step 2: Get All Users

```powershell
$headers = @{ "Authorization" = "Bearer $token" }

$users = Invoke-RestMethod -Uri "http://localhost:5000/api/users" `
  -Method Get `
  -Headers $headers

$users | ConvertTo-Json
```

## Step 3: Get User By ID

```powershell
$userId = $users[0].id

$user = Invoke-RestMethod -Uri "http://localhost:5000/api/users/$userId" `
  -Method Get `
  -Headers $headers

$user | ConvertTo-Json
```

## Step 4: Create New User

First, get the SuperAdmin role ID:

```powershell
# You need to query roles from database or use the seeded role
# For now, we'll create a user without roles
$createUserBody = @{
    username = "testuser"
    email = "test@example.com"
    password = "Test123!"
    roleIds = @()
} | ConvertTo-Json

$newUser = Invoke-RestMethod -Uri "http://localhost:5000/api/users" `
  -Method Post `
  -ContentType "application/json" `
  -Headers $headers `
  -Body $createUserBody

Write-Host "New User Created:"
$newUser | ConvertTo-Json
```

## Step 5: Update User

```powershell
$updateUserBody = @{
    username = "testuser_updated"
    email = "test@example.com"
    isActive = $true
    roleIds = @()
} | ConvertTo-Json

$updatedUser = Invoke-RestMethod -Uri "http://localhost:5000/api/users/$($newUser.id)" `
  -Method Put `
  -ContentType "application/json" `
  -Headers $headers `
  -Body $updateUserBody

Write-Host "User Updated:"
$updatedUser | ConvertTo-Json
```

## Step 6: Delete User

```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/users/$($newUser.id)" `
  -Method Delete `
  -Headers $headers

Write-Host "User Deleted Successfully"
```

## Complete Test Script

Save this as `test-api.ps1`:

```powershell
# Login
Write-Host "=== Step 1: Login ===" -ForegroundColor Green
$loginResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{"email":"admin@domain.com","password":"Admin123!"}'

$token = $loginResponse.accessToken
$headers = @{ "Authorization" = "Bearer $token" }
Write-Host "✓ Login successful" -ForegroundColor Green

# Get All Users
Write-Host "`n=== Step 2: Get All Users ===" -ForegroundColor Green
$users = Invoke-RestMethod -Uri "http://localhost:5000/api/users" `
  -Method Get `
  -Headers $headers
Write-Host "✓ Found $($users.Count) users" -ForegroundColor Green

# Create User
Write-Host "`n=== Step 3: Create User ===" -ForegroundColor Green
$createUserBody = @{
    username = "testuser"
    email = "test@example.com"
    password = "Test123!"
    roleIds = @()
} | ConvertTo-Json

$newUser = Invoke-RestMethod -Uri "http://localhost:5000/api/users" `
  -Method Post `
  -ContentType "application/json" `
  -Headers $headers `
  -Body $createUserBody
Write-Host "✓ User created: $($newUser.username)" -ForegroundColor Green

# Get User By ID
Write-Host "`n=== Step 4: Get User By ID ===" -ForegroundColor Green
$user = Invoke-RestMethod -Uri "http://localhost:5000/api/users/$($newUser.id)" `
  -Method Get `
  -Headers $headers
Write-Host "✓ User retrieved: $($user.username)" -ForegroundColor Green

# Update User
Write-Host "`n=== Step 5: Update User ===" -ForegroundColor Green
$updateUserBody = @{
    username = "testuser_updated"
    email = "test_updated@example.com"
    isActive = $true
    roleIds = @()
} | ConvertTo-Json

$updatedUser = Invoke-RestMethod -Uri "http://localhost:5000/api/users/$($newUser.id)" `
  -Method Put `
  -ContentType "application/json" `
  -Headers $headers `
  -Body $updateUserBody
Write-Host "✓ User updated: $($updatedUser.username)" -ForegroundColor Green

# Delete User
Write-Host "`n=== Step 6: Delete User ===" -ForegroundColor Green
Invoke-RestMethod -Uri "http://localhost:5000/api/users/$($newUser.id)" `
  -Method Delete `
  -Headers $headers
Write-Host "✓ User deleted successfully" -ForegroundColor Green

Write-Host "`n=== All Tests Passed! ===" -ForegroundColor Cyan
```

Run with: `.\test-api.ps1`

## Using Swagger UI

1. Open http://localhost:5000/swagger
2. Click "Authorize" button
3. Login first to get token
4. Enter: `Bearer {your-token}`
5. Try all endpoints

## Expected Responses

### Login Success
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "base64...",
  "email": "admin@domain.com",
  "username": "admin"
}
```

### Get Users Success
```json
[
  {
    "id": "guid",
    "username": "admin",
    "email": "admin@domain.com",
    "isActive": true
  }
]
```

### Create User Success (201 Created)
```json
{
  "id": "new-guid",
  "username": "testuser",
  "email": "test@example.com"
}
```

### Update User Success (200 OK)
```json
{
  "id": "guid",
  "username": "testuser_updated",
  "email": "test_updated@example.com",
  "isActive": true
}
```

### Delete User Success (204 No Content)
No response body

## Error Responses

### 401 Unauthorized
```json
{
  "message": "Invalid credentials"
}
```

### 404 Not Found
```json
{
  "message": "User not found"
}
```

### 400 Validation Error
```json
{
  "errors": {
    "Email": ["'Email' is not a valid email address."],
    "Password": ["'Password' must be at least 6 characters."]
  }
}
```

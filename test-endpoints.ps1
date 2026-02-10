# FraudRuleEngineService API Test Script
# Run with: .\test-endpoints.ps1

$baseUrl = "http://localhost:5001"

Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "FRAUD RULE ENGINE SERVICE - API TESTS" -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""

# ============================================================
# FRAUD RULES
# ============================================================
Write-Host "1. GET /api/fraud-rules - List all fraud rules" -ForegroundColor Yellow
$rules = Invoke-RestMethod -Uri "$baseUrl/api/fraud-rules" -Method Get
$rules | ConvertTo-Json -Depth 3
Write-Host ""

# ============================================================
# TRANSACTIONS - Evaluate for fraud
# ============================================================
Write-Host "2. POST /api/transactions/evaluate - High-value transaction (should trigger alert)" -ForegroundColor Yellow
$highValueTx = @{
    accountNumber = "1234567890"
    amount = 75000
    currency = "ZAR"
    merchantName = "Luxury Jewellers"
    category = "Shopping"
    transactionDate = "2025-01-15T14:30:00"
    location = "Sandton City"
    country = "ZA"
    channel = 1
} | ConvertTo-Json

$result1 = Invoke-RestMethod -Uri "$baseUrl/api/transactions/evaluate" -Method Post -Body $highValueTx -ContentType "application/json"
$result1 | ConvertTo-Json -Depth 5
Write-Host ""

Write-Host "3. POST /api/transactions/evaluate - Clean transaction (no alerts)" -ForegroundColor Yellow
$cleanTx = @{
    accountNumber = "1234567890"
    amount = 150
    currency = "ZAR"
    merchantName = "Woolworths"
    category = "Groceries"
    transactionDate = "2025-01-15T10:00:00"
    location = "Cape Town"
    country = "ZA"
    channel = 1
} | ConvertTo-Json

$result2 = Invoke-RestMethod -Uri "$baseUrl/api/transactions/evaluate" -Method Post -Body $cleanTx -ContentType "application/json"
$result2 | ConvertTo-Json -Depth 5
Write-Host ""

Write-Host "4. POST /api/transactions/evaluate - Cross-border transaction" -ForegroundColor Yellow
$crossBorderTx = @{
    accountNumber = "9876543210"
    amount = 5000
    currency = "USD"
    merchantName = "Amazon US"
    category = "Online Shopping"
    transactionDate = "2025-01-15T12:00:00"
    location = "Seattle"
    country = "US"
    channel = 0
} | ConvertTo-Json

$result3 = Invoke-RestMethod -Uri "$baseUrl/api/transactions/evaluate" -Method Post -Body $crossBorderTx -ContentType "application/json"
$result3 | ConvertTo-Json -Depth 5
Write-Host ""

# ============================================================
# FRAUD ALERTS
# ============================================================
Write-Host "5. GET /api/fraud-alerts - List all alerts" -ForegroundColor Yellow
$alerts = Invoke-RestMethod -Uri "$baseUrl/api/fraud-alerts?page=1&pageSize=20" -Method Get
$alerts | ConvertTo-Json -Depth 5
Write-Host ""

Write-Host "6. GET /api/fraud-alerts?status=Open - Filter by status" -ForegroundColor Yellow
$openAlerts = Invoke-RestMethod -Uri "$baseUrl/api/fraud-alerts?status=Open" -Method Get
$openAlerts | ConvertTo-Json -Depth 5
Write-Host ""

Write-Host "7. GET /api/fraud-alerts/statistics - Alert statistics" -ForegroundColor Yellow
$stats = Invoke-RestMethod -Uri "$baseUrl/api/fraud-alerts/statistics" -Method Get
$stats | ConvertTo-Json -Depth 5
Write-Host ""

# Review an alert if any exist
if ($alerts.items -and $alerts.items.Count -gt 0) {
    $alertRef = $alerts.items[0].alertReference

    Write-Host "8. GET /api/fraud-alerts/$alertRef - Get specific alert" -ForegroundColor Yellow
    $alert = Invoke-RestMethod -Uri "$baseUrl/api/fraud-alerts/$alertRef" -Method Get
    $alert | ConvertTo-Json -Depth 3
    Write-Host ""

    Write-Host "9. PATCH /api/fraud-alerts/$alertRef/review - Mark as Investigating" -ForegroundColor Yellow
    $reviewBody = @{
        status = 1
        reviewedBy = "analyst@capitec.co.za"
    } | ConvertTo-Json

    $reviewed = Invoke-RestMethod -Uri "$baseUrl/api/fraud-alerts/$alertRef/review" -Method Patch -Body $reviewBody -ContentType "application/json"
    $reviewed | ConvertTo-Json -Depth 3
    Write-Host ""
}

# ============================================================
# FRAUD RULES - Update configuration
# ============================================================
Write-Host "10. PUT /api/fraud-rules/HighValueTransaction - Update threshold" -ForegroundColor Yellow
$updateRule = @{
    isEnabled = $true
    parameters = '{"ThresholdAmount": 100000}'
} | ConvertTo-Json

$updatedRule = Invoke-RestMethod -Uri "$baseUrl/api/fraud-rules/HighValueTransaction" -Method Put -Body $updateRule -ContentType "application/json"
$updatedRule | ConvertTo-Json -Depth 3
Write-Host ""

Write-Host "============================================================" -ForegroundColor Green
Write-Host "ALL TESTS COMPLETED" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green

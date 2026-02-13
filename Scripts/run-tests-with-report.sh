#!/bin/bash
# Run tests and generate HTML report

set -e

# Resolve project root (works whether run from root or Scripts folder)
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# Timestamp for unique filenames
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")

echo "=============================================="
echo "  Running Tests & Generating HTML Report"
echo "=============================================="
echo ""
echo "Project: $PROJECT_ROOT"
echo "Run ID:  $TIMESTAMP"
echo ""

# Step 1: Run tests with .trx logger (timestamped)
TRX_FILE="test-results_${TIMESTAMP}.trx"
echo "Step 1: Running tests..."
dotnet test "$PROJECT_ROOT/Contentstack.Core.Tests/Contentstack.Core.Tests.csproj" \
  --filter "FullyQualifiedName~Integration" \
  --logger "trx;LogFileName=$TRX_FILE" \
  --results-directory "$PROJECT_ROOT/Contentstack.Core.Tests/TestResults" \
  --verbosity quiet || true

echo ""
echo "Tests completed!"
echo ""

# Step 2: Generate enhanced HTML report (timestamped)
REPORT_FILE="test-report-enhanced_${TIMESTAMP}.html"
echo "Step 2: Generating enhanced HTML report..."
cd "$PROJECT_ROOT"
python3 "$PROJECT_ROOT/Scripts/generate_enhanced_html_report.py" \
  "$PROJECT_ROOT/Contentstack.Core.Tests/TestResults/$TRX_FILE"

# Move timestamped report to project root if generated elsewhere
if [ -f "$PROJECT_ROOT/Contentstack.Core.Tests/$REPORT_FILE" ]; then
  mv "$PROJECT_ROOT/Contentstack.Core.Tests/$REPORT_FILE" "$PROJECT_ROOT/$REPORT_FILE" 2>/dev/null || true
fi

# Find the latest generated report (in case python script created it in cwd)
LATEST_REPORT=$(ls -t "$PROJECT_ROOT"/test-report-enhanced_*.html 2>/dev/null | head -1)

echo ""
echo "=============================================="
echo "  All Done!"
echo "=============================================="
echo ""
if [ -n "$LATEST_REPORT" ]; then
  echo "Report: $LATEST_REPORT"
  echo ""
  echo "To open:  open $LATEST_REPORT"
else
  echo "Warning: Report file not found. Check output above for errors."
fi
echo ""

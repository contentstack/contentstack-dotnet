#!/bin/bash
# Run all integration tests and generate enhanced HTML report
# Usage: ./Scripts/run-tests-with-report.sh  (run from project root)
#    or: cd Scripts && ./run-tests-with-report.sh

set -e

# Resolve project root (works whether run from root or Scripts folder)
SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

echo "=============================================="
echo "  Running Integration Tests & HTML Report"
echo "=============================================="
echo ""
echo "Project: $PROJECT_ROOT"
echo ""

# Step 1: Run tests with .trx logger
echo "Step 1: Running all integration tests..."
dotnet test "$PROJECT_ROOT/Contentstack.Core.Tests/Contentstack.Core.Tests.csproj" \
  --filter "FullyQualifiedName~Integration" \
  --logger "trx;LogFileName=test-results.trx" \
  --results-directory "$PROJECT_ROOT/Contentstack.Core.Tests/TestResults" \
  --verbosity quiet

echo ""
echo "✅ Tests completed!"
echo ""

# Step 2: Generate enhanced HTML report
echo "Step 2: Generating enhanced HTML report..."
python3 "$PROJECT_ROOT/Scripts/generate_enhanced_html_report.py" \
  "$PROJECT_ROOT/Contentstack.Core.Tests/TestResults/test-results.trx"

# Move report to project root if generated elsewhere
if [ -f "$PROJECT_ROOT/Contentstack.Core.Tests/test-report-enhanced.html" ]; then
  mv "$PROJECT_ROOT/Contentstack.Core.Tests/test-report-enhanced.html" "$PROJECT_ROOT/test-report-enhanced.html" 2>/dev/null || true
fi

echo ""
echo "=============================================="
echo "  ✅ All Done!"
echo "=============================================="
echo ""
echo "📊 Report: $PROJECT_ROOT/test-report-enhanced.html"
echo ""
echo "To open:  open $PROJECT_ROOT/test-report-enhanced.html"
echo ""

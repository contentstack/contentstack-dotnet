#!/bin/sh

#  run-unit-test-case.sh
#  Contentstack Delivery SDK
#
#  Created based on Management SDK pattern
#  Copyright © 2025 Contentstack. All rights reserved.

echo "Removing old test results..."
rm -rf "./Contentstack.Core.Unit.Tests/TestResults"

FILE_NAME="Contentstack-Delivery-DotNet-Unit-Test-Case"

echo "Running unit tests with coverage..."
dotnet test "Contentstack.Core.Unit.Tests/Contentstack.Core.Unit.Tests.csproj" \
  --logger "trx;LogFileName=Report-$FILE_NAME.trx" \
  --collect:"XPlat code coverage" \
  --settings "Contentstack.Core.Unit.Tests/runsettings.xml" \
  --verbosity minimal

echo "Test case Completed..."   

echo "Generating HTML coverage report..."

# Restore local tools if needed
dotnet tool restore > /dev/null 2>&1

# Use reportgenerator from local tools - output to root TestResults folder
dotnet tool run reportgenerator "-reports:**/**/coverage.cobertura.xml" "-targetdir:TestResults/Coverage-$FILE_NAME" "-reporttypes:HTML;HTMLSummary" "-classfilters:-*Tests*;-*Mokes*;-*Mocks*" "-filefilters:-*Tests*;-*Mokes*;-*Mocks*"

echo "HTML coverage report generated at: TestResults/Coverage-$FILE_NAME/index.html"
echo "Report generation completed!"


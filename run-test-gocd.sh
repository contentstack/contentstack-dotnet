#!/bin/bash

# Check if required environment variables are set, else exit with error
if [[ -z "$API_KEY" || -z "$DELIVERY_TOKEN" || -z "$ENVIRONMENT" ]]; then
  echo "Error: Missing environment variables. Please set API_KEY, DELIVERY_TOKEN, and ENVIRONMENT."
  exit 1
fi

# Generate XML content
xml_content="<?xml version=\"1.0\" encoding=\"utf-8\"?>
<configuration>
    <appSettings>
        <add key=\"api_key\" value=\"$API_KEY\" />
        <add key=\"delivery_token\" value=\"$DELIVERY_TOKEN\" />
        <add key=\"environment\" value=\"$ENVIRONMENT\" />
    </appSettings>
</configuration>"

# Write the XML content to app.config
echo "$xml_content" > app.config
echo "app.config file created successfully!"

# Run .NET tests
echo "Running .NET tests..."
dotnet run test

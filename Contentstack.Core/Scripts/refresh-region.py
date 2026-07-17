#!/usr/bin/env python3
# Refresh regions.json from the Contentstack CDN.
# Usage (run from your project root):
#   python3 Scripts/refresh-region.py
#   python  Scripts/refresh-region.py

import glob
import json
import os
import ssl
import sys
import urllib.request

REGIONS_URL = "https://artifacts.contentstack.com/regions.json"

print(f"Fetching {REGIONS_URL} ...")


def _fetch(url):
    # First attempt: normal SSL verification
    try:
        with urllib.request.urlopen(url, timeout=30) as r:
            return r.read().decode("utf-8")
    except urllib.error.URLError as e:
        if "CERTIFICATE_VERIFY_FAILED" not in str(e):
            raise
    # macOS python.org builds often lack system certs — retry without verification
    print("WARNING: SSL certificate verification failed. Retrying without verification.", file=sys.stderr)
    print("         To fix permanently, run: /Applications/Python*/Install\\ Certificates.command", file=sys.stderr)
    ctx = ssl.create_default_context()
    ctx.check_hostname = False
    ctx.verify_mode = ssl.CERT_NONE
    with urllib.request.urlopen(url, timeout=30, context=ctx) as r:
        return r.read().decode("utf-8")


try:
    raw = _fetch(REGIONS_URL)
except Exception as e:
    print(f"ERROR: Could not download regions.json: {e}", file=sys.stderr)
    sys.exit(1)

try:
    data = json.loads(raw)
except json.JSONDecodeError as e:
    print(f"ERROR: Downloaded content is not valid JSON: {e}", file=sys.stderr)
    sys.exit(1)

if "regions" not in data:
    print("ERROR: Downloaded JSON does not contain a 'regions' key.", file=sys.stderr)
    sys.exit(1)

region_count = len(data["regions"])

# Scan bin/ for every copy of the DLL (covers Debug/Release, any TFM, any nesting).
dll_pattern = os.path.join(os.getcwd(), "**", "Contentstack.Utils.dll")
found = [
    os.path.dirname(dll)
    for dll in glob.glob(dll_pattern, recursive=True)
    if os.sep + "bin" + os.sep in dll
]

if not found:
    print("[bin]    No build output found — run 'dotnet build' first, then re-run this script.")
    sys.exit(1)

for bin_dir in found:
    assets_dir = os.path.join(bin_dir, "Assets")
    os.makedirs(assets_dir, exist_ok=True)
    dest = os.path.join(assets_dir, "regions.json")
    with open(dest, "w", encoding="utf-8") as f:
        f.write(raw)
    print(f"[bin]    Wrote {region_count} regions → {dest}")

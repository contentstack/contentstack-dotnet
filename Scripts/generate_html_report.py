#!/usr/bin/env python3
"""
HTML Test Report Generator for .NET Test Results
Converts .trx files to beautiful HTML reports
No external dependencies - uses only Python standard library
"""

import xml.etree.ElementTree as ET
import os
import sys
from datetime import datetime
import json

class TestReportGenerator:
    def __init__(self, trx_file_path):
        self.trx_file = trx_file_path
        self.results = {
            'total': 0,
            'passed': 0,
            'failed': 0,
            'skipped': 0,
            'duration': '0s',
            'tests': []
        }
        
    def parse_trx(self):
        """Parse .trx XML file and extract test results"""
        try:
            tree = ET.parse(self.trx_file)
            root = tree.getroot()
            
            # Get namespace
            ns = {'': 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}
            
            # Get summary
            result_summary = root.find('.//ResultSummary', ns)
            counters = result_summary.find('Counters', ns) if result_summary else None
            
            if counters is not None:
                self.results['total'] = int(counters.get('total', 0))
                self.results['passed'] = int(counters.get('passed', 0))
                self.results['failed'] = int(counters.get('failed', 0))
                self.results['skipped'] = int(counters.get('notExecuted', 0))
            
            # Get test results
            test_results = root.findall('.//UnitTestResult', ns)
            
            for test_result in test_results:
                test_name = test_result.get('testName', 'Unknown')
                outcome = test_result.get('outcome', 'Unknown')
                duration = test_result.get('duration', '0')
                
                # Parse duration (format: HH:MM:SS.mmmmmmm)
                try:
                    parts = duration.split(':')
                    if len(parts) == 3:
                        hours = int(parts[0])
                        minutes = int(parts[1])
                        seconds = float(parts[2])
                        total_seconds = hours * 3600 + minutes * 60 + seconds
                        duration_str = f"{total_seconds:.2f}s"
                    else:
                        duration_str = duration
                except:
                    duration_str = duration
                
                # Get error message if failed
                error_message = None
                error_stacktrace = None
                output_elem = test_result.find('Output', ns)
                if output_elem is not None:
                    error_info = output_elem.find('ErrorInfo', ns)
                    if error_info is not None:
                        message_elem = error_info.find('Message', ns)
                        stacktrace_elem = error_info.find('StackTrace', ns)
                        if message_elem is not None:
                            error_message = message_elem.text
                        if stacktrace_elem is not None:
                            error_stacktrace = stacktrace_elem.text
                
                # Get test category
                test_def_id = test_result.get('testId', '')
                test_def = root.find(f".//UnitTest[@id='{test_def_id}']", ns)
                category = 'General'
                if test_def is not None:
                    test_method = test_def.find('.//TestMethod', ns)
                    if test_method is not None:
                        class_name = test_method.get('className', '')
                        # Extract category from namespace
                        if 'Integration' in class_name:
                            parts = class_name.split('.')
                            if len(parts) >= 5:
                                category = parts[4]  # e.g., "QueryTests", "EntryTests"
                
                self.results['tests'].append({
                    'name': test_name,
                    'outcome': outcome,
                    'duration': duration_str,
                    'category': category,
                    'error_message': error_message,
                    'error_stacktrace': error_stacktrace
                })
            
            return True
            
        except Exception as e:
            print(f"Error parsing TRX file: {e}")
            return False
    
    def generate_html(self, output_file='test-report.html'):
        """Generate beautiful HTML report"""
        
        # Calculate pass rate
        pass_rate = (self.results['passed'] / self.results['total'] * 100) if self.results['total'] > 0 else 0
        
        # Group tests by category
        tests_by_category = {}
        for test in self.results['tests']:
            category = test['category']
            if category not in tests_by_category:
                tests_by_category[category] = []
            tests_by_category[category].append(test)
        
        # Sort categories
        sorted_categories = sorted(tests_by_category.keys())
        
        html = f"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>.NET CDA SDK - Test Report</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
            color: #333;
        }}
        
        .container {{
            max-width: 1400px;
            margin: 0 auto;
            background: white;
            border-radius: 12px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            overflow: hidden;
        }}
        
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px;
            text-align: center;
        }}
        
        .header h1 {{
            font-size: 2.5em;
            margin-bottom: 10px;
            font-weight: 700;
        }}
        
        .header p {{
            font-size: 1.1em;
            opacity: 0.9;
        }}
        
        .summary {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            padding: 40px;
            background: #f8f9fa;
        }}
        
        .summary-card {{
            background: white;
            padding: 25px;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            text-align: center;
            transition: transform 0.2s;
        }}
        
        .summary-card:hover {{
            transform: translateY(-5px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }}
        
        .summary-card .number {{
            font-size: 3em;
            font-weight: bold;
            margin-bottom: 10px;
        }}
        
        .summary-card .label {{
            font-size: 0.9em;
            color: #666;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}
        
        .passed {{ color: #28a745; }}
        .failed {{ color: #dc3545; }}
        .skipped {{ color: #ffc107; }}
        .total {{ color: #007bff; }}
        
        .pass-rate {{
            padding: 30px 40px;
            background: white;
            text-align: center;
            border-top: 3px solid #f8f9fa;
        }}
        
        .pass-rate-bar {{
            width: 100%;
            height: 40px;
            background: #e9ecef;
            border-radius: 20px;
            overflow: hidden;
            margin: 20px 0;
            position: relative;
        }}
        
        .pass-rate-fill {{
            height: 100%;
            background: linear-gradient(90deg, #28a745 0%, #20c997 100%);
            transition: width 1s ease-out;
            display: flex;
            align-items: center;
            justify-content: center;
            color: white;
            font-weight: bold;
            font-size: 1.1em;
        }}
        
        .test-results {{
            padding: 40px;
        }}
        
        .category {{
            margin-bottom: 30px;
        }}
        
        .category-header {{
            background: #f8f9fa;
            padding: 15px 20px;
            border-left: 4px solid #667eea;
            margin-bottom: 15px;
            border-radius: 4px;
            cursor: pointer;
            display: flex;
            justify-content: space-between;
            align-items: center;
            transition: background 0.2s;
        }}
        
        .category-header:hover {{
            background: #e9ecef;
        }}
        
        .category-title {{
            font-size: 1.3em;
            font-weight: 600;
            color: #333;
        }}
        
        .category-stats {{
            font-size: 0.9em;
            color: #666;
        }}
        
        .test-table {{
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
            background: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        
        .test-table thead {{
            background: #667eea;
            color: white;
        }}
        
        .test-table th {{
            padding: 15px;
            text-align: left;
            font-weight: 600;
            text-transform: uppercase;
            font-size: 0.85em;
            letter-spacing: 0.5px;
        }}
        
        .test-table td {{
            padding: 15px;
            border-bottom: 1px solid #e9ecef;
        }}
        
        .test-table tr:last-child td {{
            border-bottom: none;
        }}
        
        .test-table tbody tr:hover {{
            background: #f8f9fa;
        }}
        
        .test-name {{
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.9em;
        }}
        
        .status-badge {{
            display: inline-block;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 0.85em;
            font-weight: 600;
            text-transform: uppercase;
        }}
        
        .status-passed {{
            background: #d4edda;
            color: #155724;
        }}
        
        .status-failed {{
            background: #f8d7da;
            color: #721c24;
        }}
        
        .status-skipped {{
            background: #fff3cd;
            color: #856404;
        }}
        
        .error-details {{
            background: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 15px;
            margin-top: 10px;
            border-radius: 4px;
        }}
        
        .error-message {{
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.9em;
            color: #721c24;
            white-space: pre-wrap;
            margin-bottom: 10px;
        }}
        
        .error-stacktrace {{
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.8em;
            color: #666;
            white-space: pre-wrap;
            background: #f8f9fa;
            padding: 10px;
            border-radius: 4px;
            max-height: 300px;
            overflow-y: auto;
        }}
        
        .footer {{
            background: #f8f9fa;
            padding: 30px;
            text-align: center;
            color: #666;
            border-top: 3px solid #e9ecef;
        }}
        
        .footer p {{
            margin: 5px 0;
        }}
        
        .collapsible {{
            display: none;
        }}
        
        .collapsible.active {{
            display: block;
        }}
        
        .toggle-icon {{
            transition: transform 0.3s;
        }}
        
        .toggle-icon.rotated {{
            transform: rotate(90deg);
        }}
        
        @media print {{
            body {{
                background: white;
                padding: 0;
            }}
            
            .container {{
                box-shadow: none;
            }}
            
            .category-header {{
                cursor: default;
            }}
            
            .collapsible {{
                display: block !important;
            }}
        }}
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>.NET CDA SDK Test Report</h1>
            <p>Integration Test Results - {datetime.now().strftime('%B %d, %Y at %I:%M %p')}</p>
        </div>
        
        <div class="summary">
            <div class="summary-card">
                <div class="number total">{self.results['total']}</div>
                <div class="label">Total Tests</div>
            </div>
            <div class="summary-card">
                <div class="number passed">{self.results['passed']}</div>
                <div class="label">Passed</div>
            </div>
            <div class="summary-card">
                <div class="number failed">{self.results['failed']}</div>
                <div class="label">Failed</div>
            </div>
            <div class="summary-card">
                <div class="number skipped">{self.results['skipped']}</div>
                <div class="label">Skipped</div>
            </div>
        </div>
        
        <div class="pass-rate">
            <h2>Pass Rate</h2>
            <div class="pass-rate-bar">
                <div class="pass-rate-fill" style="width: {pass_rate}%">
                    {pass_rate:.1f}%
                </div>
            </div>
        </div>
        
        <div class="test-results">
            <h2 style="margin-bottom: 30px; font-size: 2em;">Test Results by Category</h2>
"""
        
        # Generate category sections
        for category in sorted_categories:
            tests = tests_by_category[category]
            passed = sum(1 for t in tests if t['outcome'] == 'Passed')
            failed = sum(1 for t in tests if t['outcome'] == 'Failed')
            skipped = sum(1 for t in tests if t['outcome'] == 'NotExecuted')
            
            html += f"""
            <div class="category">
                <div class="category-header" onclick="toggleCategory('{category}')">
                    <div>
                        <span class="toggle-icon" id="icon-{category}">▶</span>
                        <span class="category-title">{category}</span>
                    </div>
                    <div class="category-stats">
                        <span class="passed">{passed} passed</span> · 
                        <span class="failed">{failed} failed</span> · 
                        <span class="skipped">{skipped} skipped</span> · 
                        <span>{len(tests)} total</span>
                    </div>
                </div>
                
                <div id="{category}" class="collapsible">
                    <table class="test-table">
                        <thead>
                            <tr>
                                <th style="width: 50%">Test Name</th>
                                <th style="width: 20%">Status</th>
                                <th style="width: 15%">Duration</th>
                            </tr>
                        </thead>
                        <tbody>
"""
            
            for test in tests:
                status_class = 'status-passed' if test['outcome'] == 'Passed' else 'status-failed' if test['outcome'] == 'Failed' else 'status-skipped'
                
                html += f"""
                            <tr>
                                <td>
                                    <div class="test-name">{test['name']}</div>
"""
                
                # Add error details if failed
                if test['outcome'] == 'Failed' and (test['error_message'] or test['error_stacktrace']):
                    html += """
                                    <div class="error-details">
"""
                    if test['error_message']:
                        html += f"""
                                        <div class="error-message"><strong>Error:</strong><br>{self.escape_html(test['error_message'])}</div>
"""
                    if test['error_stacktrace']:
                        html += f"""
                                        <details>
                                            <summary style="cursor: pointer; font-weight: bold; margin-bottom: 10px;">Stack Trace</summary>
                                            <div class="error-stacktrace">{self.escape_html(test['error_stacktrace'])}</div>
                                        </details>
"""
                    html += """
                                    </div>
"""
                
                html += f"""
                                </td>
                                <td><span class="status-badge {status_class}">{test['outcome']}</span></td>
                                <td>{test['duration']}</td>
                            </tr>
"""
            
            html += """
                        </tbody>
                    </table>
                </div>
            </div>
"""
        
        html += f"""
        </div>
        
        <div class="footer">
            <p><strong>.NET CDA SDK Integration Tests</strong></p>
            <p>Generated on {datetime.now().strftime('%Y-%m-%d at %H:%M:%S')}</p>
            <p>Report Version 1.0</p>
        </div>
    </div>
    
    <script>
        function toggleCategory(categoryId) {{
            const element = document.getElementById(categoryId);
            const icon = document.getElementById('icon-' + categoryId);
            
            if (element.classList.contains('active')) {{
                element.classList.remove('active');
                icon.classList.remove('rotated');
            }} else {{
                element.classList.add('active');
                icon.classList.add('rotated');
            }}
        }}
        
        // Auto-expand failed tests
        document.addEventListener('DOMContentLoaded', function() {{
            const categories = document.querySelectorAll('.category');
            categories.forEach(category => {{
                const stats = category.querySelector('.category-stats');
                if (stats && stats.textContent.includes('failed') && !stats.textContent.includes('0 failed')) {{
                    const categoryId = category.querySelector('.collapsible').id;
                    toggleCategory(categoryId);
                }}
            }});
        }});
    </script>
</body>
</html>
"""
        
        # Write HTML file
        with open(output_file, 'w', encoding='utf-8') as f:
            f.write(html)
        
        print(f"✅ HTML report generated: {output_file}")
        return output_file
    
    def escape_html(self, text):
        """Escape HTML special characters"""
        if text is None:
            return ""
        return (text
                .replace('&', '&amp;')
                .replace('<', '&lt;')
                .replace('>', '&gt;')
                .replace('"', '&quot;')
                .replace("'", '&#39;'))


def main():
    """Main entry point"""
    print("="*80)
    print("🧪 .NET Test Report Generator")
    print("="*80)
    
    # Check for .trx file
    trx_file = None
    
    if len(sys.argv) > 1:
        trx_file = sys.argv[1]
    else:
        # Look for .trx files in TestResults directory
        test_results_dir = './TestResults'
        if os.path.exists(test_results_dir):
            trx_files = [f for f in os.listdir(test_results_dir) if f.endswith('.trx')]
            if trx_files:
                trx_file = os.path.join(test_results_dir, trx_files[0])
    
    if not trx_file or not os.path.exists(trx_file):
        print("\n❌ Error: No .trx file found!")
        print("\nUsage:")
        print("  python3 generate_html_report.py <path-to-trx-file>")
        print("\nOr run tests first to generate .trx file:")
        print("  dotnet test --logger 'trx;LogFileName=test-results.trx' --results-directory ./TestResults")
        sys.exit(1)
    
    print(f"\n📄 Input file: {trx_file}")
    
    # Generate report
    generator = TestReportGenerator(trx_file)
    
    print("\n⏳ Parsing test results...")
    if not generator.parse_trx():
        print("❌ Failed to parse TRX file")
        sys.exit(1)
    
    print(f"✅ Found {generator.results['total']} tests")
    print(f"   • Passed: {generator.results['passed']}")
    print(f"   • Failed: {generator.results['failed']}")
    print(f"   • Skipped: {generator.results['skipped']}")
    
    print("\n⏳ Generating HTML report...")
    output_file = generator.generate_html('test-report.html')
    
    print("\n" + "="*80)
    print("✅ SUCCESS! HTML report generated")
    print("="*80)
    print(f"\n📊 Open the report: {os.path.abspath(output_file)}")
    print("\nIn your browser:")
    print(f"  file://{os.path.abspath(output_file)}")
    
    # Summary
    print("\n📋 Summary:")
    print(f"  Total Tests:  {generator.results['total']}")
    print(f"  Passed:       {generator.results['passed']} ({generator.results['passed']/generator.results['total']*100:.1f}%)")
    print(f"  Failed:       {generator.results['failed']}")
    print(f"  Skipped:      {generator.results['skipped']}")
    
    print("\n🎉 Done!")


if __name__ == "__main__":
    main()


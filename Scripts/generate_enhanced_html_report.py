#!/usr/bin/env python3
"""
Enhanced HTML Test Report Generator for .NET Test Results
Converts .trx files to beautiful HTML reports with:
- Expected vs Actual values
- HTTP Request details (including cURL)
- Response details
No external dependencies - uses only Python standard library
"""

import xml.etree.ElementTree as ET
import os
import sys
import re
import json
from datetime import datetime

class EnhancedTestReportGenerator:
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
        
    def parse_structured_output(self, output_text):
        """Parse structured test output (assertions, requests, responses)"""
        if not output_text:
            return {
                'assertions': [],
                'requests': [],
                'responses': [],
                'context': [],
                'steps': []
            }
        
        structured_data = {
            'assertions': [],
            'requests': [],
            'responses': [],
            'context': [],
            'steps': []
        }
        
        # Find all structured outputs
        pattern = r'###TEST_OUTPUT_START###(.+?)###TEST_OUTPUT_END###'
        matches = re.findall(pattern, output_text, re.DOTALL)
        
        for match in matches:
            try:
                data = json.loads(match)
                output_type = data.get('type', '').upper()
                
                if output_type == 'ASSERTION':
                    structured_data['assertions'].append({
                        'name': data.get('assertionName', 'Unknown'),
                        'expected': data.get('expected', 'N/A'),
                        'actual': data.get('actual', 'N/A'),
                        'passed': data.get('passed', True)
                    })
                elif output_type == 'HTTP_REQUEST':
                    structured_data['requests'].append({
                        'method': data.get('method', 'GET'),
                        'url': data.get('url', ''),
                        'headers': data.get('headers', {}),
                        'body': data.get('body', ''),
                        'curl': data.get('curlCommand', '')
                    })
                elif output_type == 'HTTP_RESPONSE':
                    structured_data['responses'].append({
                        'statusCode': data.get('statusCode', 0),
                        'statusText': data.get('statusText', ''),
                        'headers': data.get('headers', {}),
                        'body': data.get('body', '')
                    })
                elif output_type == 'CONTEXT':
                    structured_data['context'].append({
                        'key': data.get('key', ''),
                        'value': data.get('value', '')
                    })
                elif output_type == 'STEP':
                    structured_data['steps'].append({
                        'name': data.get('stepName', ''),
                        'description': data.get('description', '')
                    })
            except json.JSONDecodeError:
                continue
        
        return structured_data
        
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
                test_output = None
                structured_output = None
                
                output_elem = test_result.find('Output', ns)
                if output_elem is not None:
                    # Get error info
                    error_info = output_elem.find('ErrorInfo', ns)
                    if error_info is not None:
                        message_elem = error_info.find('Message', ns)
                        stacktrace_elem = error_info.find('StackTrace', ns)
                        if message_elem is not None:
                            error_message = message_elem.text
                        if stacktrace_elem is not None:
                            error_stacktrace = stacktrace_elem.text
                    
                    # Get standard output (contains our structured data)
                    stdout_elem = output_elem.find('StdOut', ns)
                    if stdout_elem is not None and stdout_elem.text:
                        test_output = stdout_elem.text
                        structured_output = self.parse_structured_output(test_output)
                
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
                    'error_stacktrace': error_stacktrace,
                    'structured_output': structured_output
                })
            
            return True
            
        except Exception as e:
            print(f"Error parsing TRX file: {e}")
            import traceback
            traceback.print_exc()
            return False
    
    def generate_test_details_html(self, test):
        """Generate detailed HTML for a single test including assertions, requests, responses"""
        html = ""
        
        if not test.get('structured_output'):
            return html
        
        structured = test['structured_output']
        
        # Test Steps
        if structured.get('steps'):
            html += """
                <div class="test-details-section">
                    <h4 class="details-heading">📝 Test Steps</h4>
                    <div class="steps-list">
"""
            for step in structured['steps']:
                html += f"""
                        <div class="step-item">
                            <strong>{self.escape_html(step['name'])}</strong>
"""
                if step.get('description'):
                    html += f"""<p>{self.escape_html(step['description'])}</p>"""
                html += """
                        </div>
"""
            html += """
                    </div>
                </div>
"""
        
        # Assertions (Expected vs Actual)
        if structured.get('assertions'):
            html += """
                <div class="test-details-section">
                    <h4 class="details-heading">✓ Assertions</h4>
                    <div class="assertions-table">
"""
            for assertion in structured['assertions']:
                status_icon = "✅" if assertion.get('passed', True) else "❌"
                status_class = "passed" if assertion.get('passed', True) else "failed"
                
                html += f"""
                        <div class="assertion-row {status_class}">
                            <div class="assertion-header">
                                <span class="assertion-icon">{status_icon}</span>
                                <strong>{self.escape_html(assertion['name'])}</strong>
                            </div>
                            <div class="assertion-comparison">
                                <div class="expected-actual-row">
                                    <div class="expected-col">
                                        <span class="label">Expected:</span>
                                        <pre class="value-box">{self.escape_html(str(assertion['expected']))}</pre>
                                    </div>
                                    <div class="actual-col">
                                        <span class="label">Actual:</span>
                                        <pre class="value-box">{self.escape_html(str(assertion['actual']))}</pre>
                                    </div>
                                </div>
                            </div>
                        </div>
"""
            html += """
                    </div>
                </div>
"""
        
        # HTTP Requests (with cURL)
        if structured.get('requests'):
            html += """
                <div class="test-details-section">
                    <h4 class="details-heading">🌐 HTTP Requests</h4>
"""
            for i, request in enumerate(structured['requests']):
                html += f"""
                    <div class="request-block">
                        <div class="request-summary">
                            <span class="http-method">{self.escape_html(request['method'])}</span>
                            <span class="http-url">{self.escape_html(request['url'])}</span>
                        </div>
"""
                
                # Request Headers
                if request.get('headers'):
                    html += """
                        <details class="request-details">
                            <summary>📋 Request Headers</summary>
                            <pre class="headers-box">"""
                    for key, value in request['headers'].items():
                        html += f"{self.escape_html(key)}: {self.escape_html(value)}\n"
                    html += """</pre>
                        </details>
"""
                
                # Request Body
                if request.get('body'):
                    html += f"""
                        <details class="request-details">
                            <summary>📦 Request Body</summary>
                            <pre class="body-box">{self.escape_html(request['body'])}</pre>
                        </details>
"""
                
                # cURL Command
                if request.get('curl'):
                    html += f"""
                        <details class="request-details curl-section">
                            <summary>🔧 cURL Command</summary>
                            <pre class="curl-box">{self.escape_html(request['curl'])}</pre>
                            <button class="copy-btn" onclick="copyToClipboard('curl-{i}')">📋 Copy</button>
                            <textarea id="curl-{i}" style="position:absolute;left:-9999px;">{self.escape_html(request['curl'])}</textarea>
                        </details>
"""
                
                html += """
                    </div>
"""
            html += """
                </div>
"""
        
        # HTTP Responses
        if structured.get('responses'):
            html += """
                <div class="test-details-section">
                    <h4 class="details-heading">📥 HTTP Responses</h4>
"""
            for response in structured['responses']:
                status_class = "success" if 200 <= response.get('statusCode', 0) < 300 else "error"
                html += f"""
                    <div class="response-block">
                        <div class="response-summary {status_class}">
                            <span class="http-status">{response.get('statusCode', 'N/A')} {self.escape_html(response.get('statusText', ''))}</span>
                        </div>
"""
                
                # Response Headers
                if response.get('headers'):
                    html += """
                        <details class="response-details">
                            <summary>📋 Response Headers</summary>
                            <pre class="headers-box">"""
                    for key, value in response['headers'].items():
                        html += f"{self.escape_html(key)}: {self.escape_html(value)}\n"
                    html += """</pre>
                        </details>
"""
                
                # Response Body
                if response.get('body'):
                    html += f"""
                        <details class="response-details">
                            <summary>📦 Response Body</summary>
                            <pre class="body-box">{self.escape_html(response['body'][:3000])}</pre>
                        </details>
"""
                
                html += """
                    </div>
"""
            html += """
                </div>
"""
        
        # Context Information
        if structured.get('context'):
            html += """
                <div class="test-details-section">
                    <h4 class="details-heading">ℹ️ Context</h4>
                    <div class="context-list">
"""
            for ctx in structured['context']:
                html += f"""
                        <div class="context-item">
                            <strong>{self.escape_html(ctx['key'])}:</strong>
                            <pre class="context-value">{self.escape_html(str(ctx['value']))}</pre>
                        </div>
"""
            html += """
                    </div>
                </div>
"""
        
        return html
    
    def escape_html(self, text):
        """Escape HTML special characters"""
        if text is None:
            return ""
        text = str(text)
        return (text
                .replace('&', '&amp;')
                .replace('<', '&lt;')
                .replace('>', '&gt;')
                .replace('"', '&quot;')
                .replace("'", '&#39;'))
    
    def generate_html(self, output_file='test-report-enhanced.html'):
        """Generate enhanced HTML report"""
        
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
    <title>.NET CDA SDK - Enhanced Test Report</title>
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
            max-width: 1600px;
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
            cursor: pointer;
            color: #007bff;
        }}
        
        .test-name:hover {{
            text-decoration: underline;
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
        
        /* Enhanced Test Details Styles */
        .test-details-container {{
            background: #f8f9fa;
            border-left: 4px solid #007bff;
            margin-top: 15px;
            padding: 20px;
            border-radius: 4px;
            display: none;
        }}
        
        .test-details-container.show {{
            display: block;
        }}
        
        .test-details-section {{
            margin-bottom: 25px;
        }}
        
        .details-heading {{
            font-size: 1.1em;
            color: #333;
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 2px solid #e9ecef;
        }}
        
        /* Assertions Styles */
        .assertions-table {{
            background: white;
            border-radius: 6px;
            overflow: hidden;
        }}
        
        .assertion-row {{
            padding: 15px;
            border-bottom: 1px solid #e9ecef;
        }}
        
        .assertion-row:last-child {{
            border-bottom: none;
        }}
        
        .assertion-row.failed {{
            background: #fff5f5;
        }}
        
        .assertion-header {{
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 10px;
        }}
        
        .assertion-icon {{
            font-size: 1.2em;
        }}
        
        .expected-actual-row {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
            margin-top: 10px;
        }}
        
        .expected-col, .actual-col {{
            background: white;
        }}
        
        .label {{
            font-weight: 600;
            color: #666;
            font-size: 0.85em;
            text-transform: uppercase;
            display: block;
            margin-bottom: 5px;
        }}
        
        .value-box {{
            background: #f8f9fa;
            padding: 10px;
            border-radius: 4px;
            border: 1px solid #dee2e6;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.85em;
            max-height: 200px;
            overflow-y: auto;
            white-space: pre-wrap;
            word-break: break-all;
        }}
        
        /* HTTP Request/Response Styles */
        .request-block, .response-block {{
            background: white;
            padding: 15px;
            border-radius: 6px;
            margin-bottom: 15px;
            border: 1px solid #dee2e6;
        }}
        
        .request-summary, .response-summary {{
            display: flex;
            align-items: center;
            gap: 10px;
            margin-bottom: 10px;
        }}
        
        .http-method {{
            background: #007bff;
            color: white;
            padding: 4px 10px;
            border-radius: 4px;
            font-weight: 600;
            font-size: 0.85em;
        }}
        
        .http-url {{
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.9em;
            color: #333;
            word-break: break-all;
        }}
        
        .http-status {{
            font-weight: 600;
            font-size: 1.1em;
        }}
        
        .response-summary.success .http-status {{
            color: #28a745;
        }}
        
        .response-summary.error .http-status {{
            color: #dc3545;
        }}
        
        .request-details, .response-details {{
            margin-top: 10px;
        }}
        
        .request-details summary, .response-details summary {{
            cursor: pointer;
            padding: 8px 12px;
            background: #f8f9fa;
            border-radius: 4px;
            font-weight: 600;
            font-size: 0.9em;
            user-select: none;
        }}
        
        .request-details summary:hover, .response-details summary:hover {{
            background: #e9ecef;
        }}
        
        .headers-box, .body-box, .curl-box {{
            background: #f8f9fa;
            padding: 12px;
            border-radius: 4px;
            border: 1px solid #dee2e6;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.85em;
            margin-top: 10px;
            max-height: 300px;
            overflow-y: auto;
            white-space: pre-wrap;
            word-break: break-all;
        }}
        
        .curl-section {{
            background: #f0f7ff;
            border-left: 3px solid #007bff;
        }}
        
        .copy-btn {{
            background: #007bff;
            color: white;
            border: none;
            padding: 6px 12px;
            border-radius: 4px;
            cursor: pointer;
            font-size: 0.85em;
            margin-top: 5px;
            transition: background 0.2s;
        }}
        
        .copy-btn:hover {{
            background: #0056b3;
        }}
        
        /* Context and Steps */
        .context-list, .steps-list {{
            background: white;
            padding: 15px;
            border-radius: 6px;
        }}
        
        .context-item, .step-item {{
            padding: 10px;
            border-bottom: 1px solid #e9ecef;
        }}
        
        .context-item:last-child, .step-item:last-child {{
            border-bottom: none;
        }}
        
        .context-value {{
            background: #f8f9fa;
            padding: 8px;
            border-radius: 4px;
            font-family: 'Consolas', 'Monaco', monospace;
            font-size: 0.85em;
            margin-top: 5px;
        }}
        
        /* Error Details */
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
            
            .test-details-container {{
                display: block !important;
            }}
        }}
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>.NET CDA SDK Test Report</h1>
            <p>Enhanced Integration Test Results - {datetime.now().strftime('%B %d, %Y at %I:%M %p')}</p>
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
            
            for test_idx, test in enumerate(tests):
                status_class = 'status-passed' if test['outcome'] == 'Passed' else 'status-failed' if test['outcome'] == 'Failed' else 'status-skipped'
                test_id = f"test-{category}-{test_idx}"
                
                html += f"""
                            <tr>
                                <td>
                                    <div class="test-name" onclick="toggleTestDetails('{test_id}')">{test['name']}</div>
"""
                
                # Add enhanced test details
                details_html = self.generate_test_details_html(test)
                if details_html or test.get('error_message') or test.get('error_stacktrace'):
                    html += f"""
                                    <div id="{test_id}" class="test-details-container">
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
                    
                    # Add enhanced details
                    html += details_html
                    
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
            <p><strong>.NET CDA SDK Integration Tests - Enhanced Report</strong></p>
            <p>Generated on {datetime.now().strftime('%Y-%m-%d at %H:%M:%S')}</p>
            <p>Report Version 2.0 (with Expected/Actual, Requests, Responses)</p>
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
        
        function toggleTestDetails(testId) {{
            const element = document.getElementById(testId);
            if (element) {{
                element.classList.toggle('show');
            }}
        }}
        
        function copyToClipboard(elementId) {{
            const element = document.getElementById(elementId);
            if (element) {{
                element.select();
                document.execCommand('copy');
                alert('Copied to clipboard!');
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
        
        print(f"✅ Enhanced HTML report generated: {output_file}")
        return output_file


def main():
    """Main entry point"""
    print("="*80)
    print("🧪 .NET Enhanced Test Report Generator")
    print("="*80)
    print("Features: Expected/Actual, HTTP Requests, cURL, Responses")
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
        print("  python3 generate_enhanced_html_report.py <path-to-trx-file>")
        print("\nOr run tests first to generate .trx file:")
        print("  dotnet test --logger 'trx;LogFileName=test-results.trx' --results-directory ./TestResults")
        sys.exit(1)
    
    print(f"\n📄 Input file: {trx_file}")
    
    # Generate report
    generator = EnhancedTestReportGenerator(trx_file)
    
    print("\n⏳ Parsing test results...")
    if not generator.parse_trx():
        print("❌ Failed to parse TRX file")
        sys.exit(1)
    
    print(f"✅ Found {generator.results['total']} tests")
    print(f"   • Passed: {generator.results['passed']}")
    print(f"   • Failed: {generator.results['failed']}")
    print(f"   • Skipped: {generator.results['skipped']}")
    
    print("\n⏳ Generating enhanced HTML report...")
    output_file = generator.generate_html('test-report-enhanced.html')
    
    print("\n" + "="*80)
    print("✅ SUCCESS! Enhanced HTML report generated")
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

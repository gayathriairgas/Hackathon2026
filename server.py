import http.server
import socketserver
import os
import json
from urllib.parse import urlparse, parse_qs

PORT = 5000
BASE_DIR = os.path.dirname(os.path.abspath(__file__))

class LoopModernizationHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header('Access-Control-Allow-Origin', '*')
        self.send_header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS')
        self.send_header('Access-Control-Allow-Headers', 'Content-Type')
        super().end_headers()

    def do_OPTIONS(self):
        self.send_response(200)
        self.end_headers()

    def do_GET(self):
        parsed = urlparse(self.path)
        path = parsed.path

        # Route root to src/index.html
        if path == "/" or path == "/workspace" or path == "/legacy-loop" or path == "/admin":
            index_path = os.path.join(BASE_DIR, "src", "index.html")
            if os.path.exists(index_path):
                self.send_response(200)
                self.send_header("Content-type", "text/html; charset=utf-8")
                self.end_headers()
                with open(index_path, "rb") as f:
                    self.wfile.write(f.read())
                return

        # Data JSON endpoints
        if path.startswith("/data/") or path.startswith("/api/"):
            clean_name = path.replace("/api/", "").replace("/data/", "")
            if not clean_name.endswith(".json"):
                clean_name = clean_name + ".json"
            json_file = os.path.join(BASE_DIR, "data", clean_name)
            if os.path.exists(json_file):
                self.send_response(200)
                self.send_header("Content-type", "application/json")
                self.end_headers()
                with open(json_file, "rb") as f:
                    self.wfile.write(f.read())
                return

        # Fallback to default file serving
        return super().do_GET()

    def do_POST(self):
        parsed = urlparse(self.path)
        path = parsed.path

        content_length = int(self.headers.get('Content-Length', 0))
        post_data = self.rfile.read(content_length)

        if path == "/api/facilities/slot":
            try:
                body = json.loads(post_data.decode('utf-8'))
                fac_path = os.path.join(BASE_DIR, "data", "facilities.json")
                if os.path.exists(fac_path):
                    with open(fac_path, "r", encoding="utf-8") as f:
                        facilities = json.load(f)
                    
                    for fac in facilities:
                        if fac.get("id") == body.get("facilityId"):
                            for res in fac.get("resources", []):
                                if res.get("id") == body.get("resourceId"):
                                    for slot in res.get("slots", []):
                                        if slot.get("time") == body.get("time"):
                                            slot["status"] = "booked" if body.get("action") == "book" else "available"
                                            slot["bookedBy"] = body.get("userId") if body.get("action") == "book" else None

                    with open(fac_path, "w", encoding="utf-8") as f:
                        json.dump(facilities, f, indent=2)

                self.send_response(200)
                self.send_header("Content-type", "application/json")
                self.end_headers()
                self.wfile.write(json.dumps({"success": True}).encode('utf-8'))
                return
            except Exception as e:
                self.send_response(500)
                self.end_headers()
                self.wfile.write(str(e).encode('utf-8'))
                return

        self.send_response(404)
        self.end_headers()

if __name__ == "__main__":
    os.chdir(BASE_DIR)
    with socketserver.TCPServer(("", PORT), LoopModernizationHandler) as httpd:
        print(f"Air Liquide Loop Modernization live at: http://localhost:{PORT}")
        httpd.serve_forever()

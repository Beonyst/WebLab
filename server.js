const http = require('http');
const fs = require('fs');
const path = require('path');

const PORT = 8080;
const WWWROOT = path.join(__dirname, 'wwwroot');

const mimeTypes = {
    '.html': 'text/html',
    '.css': 'text/css',
    '.js': 'application/javascript',
    '.json': 'application/json',
    '.png': 'image/png',
    '.jpg': 'image/jpeg',
    '.gif': 'image/gif',
    '.svg': 'image/svg+xml',
    '.ico': 'image/x-icon'
};

const server = http.createServer((req, res) => {
    console.log(`${new Date().toISOString()} - ${req.method} ${req.url}`);

    // Parse URL
    let urlPath = req.url.split('?')[0];

    // Default to index.html for root
    if (urlPath === '/') {
        urlPath = '/index.html';
    }

    // Build file path
    const filePath = path.join(WWWROOT, urlPath);

    // Security check - ensure we don't escape wwwroot
    const resolvedPath = path.resolve(filePath);
    if (!resolvedPath.startsWith(path.resolve(WWWROOT))) {
        res.writeHead(403);
        res.end('Forbidden');
        return;
    }

    // Get file extension
    const ext = path.extname(filePath).toLowerCase();
    const contentType = mimeTypes[ext] || 'application/octet-stream';

    // Read and serve file
    fs.readFile(filePath, (err, data) => {
        if (err) {
            if (err.code === 'ENOENT') {
                res.writeHead(404);
                res.end('Not Found: ' + urlPath);
            } else {
                res.writeHead(500);
                res.end('Server Error: ' + err.code);
            }
        } else {
            res.writeHead(200, { 'Content-Type': contentType });
            res.end(data);
        }
    });
});

server.listen(PORT, () => {
    console.log(`Server running at http://localhost:${PORT}/`);
    console.log(`Serving static files from ${WWWROOT}`);
});

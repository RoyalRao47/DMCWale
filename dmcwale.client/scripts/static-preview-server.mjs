import { createServer } from 'node:http';
import { readFile } from 'node:fs/promises';
import path from 'node:path';

const port = Number(process.argv[2] || 5173);
const host = '127.0.0.1';
const root = path.join(process.cwd(), 'dist');
const contentTypes = new Map([
    ['.html', 'text/html; charset=utf-8'],
    ['.js', 'text/javascript; charset=utf-8'],
    ['.css', 'text/css; charset=utf-8'],
    ['.jpg', 'image/jpeg'],
    ['.jpeg', 'image/jpeg'],
    ['.png', 'image/png'],
    ['.svg', 'image/svg+xml']
]);

function getSafePath(url) {
    const parsedUrl = new URL(url, `http://${host}:${port}`);
    const requestedPath = parsedUrl.pathname === '/' ? '/index.html' : parsedUrl.pathname;
    const filePath = path.normalize(path.join(root, requestedPath));

    if (!filePath.startsWith(root)) {
        return null;
    }

    return filePath;
}

createServer(async (request, response) => {
    const filePath = getSafePath(request.url || '/');

    if (!filePath) {
        response.writeHead(403);
        response.end('Forbidden');
        return;
    }

    try {
        const data = await readFile(filePath);
        response.writeHead(200, {
            'Content-Type': contentTypes.get(path.extname(filePath)) || 'application/octet-stream'
        });
        response.end(data);
    } catch {
        const data = await readFile(path.join(root, 'index.html'));
        response.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
        response.end(data);
    }
}).listen(port, host, () => {
    console.log(`React preview available at http://${host}:${port}/theme-preview`);
});

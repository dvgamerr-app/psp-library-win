// Run by the Pages workflow after assembling _site, only when PUBLIC_TRACEWAY_DSN is set.
const dsn = process.env.PUBLIC_TRACEWAY_DSN
if (!dsn || !dsn.includes('@')) {
  console.error('PUBLIC_TRACEWAY_DSN must look like <token>@<instance>/api/report')
  process.exit(1)
}

const escaped = dsn.replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;')
const tags = `<meta name="traceway-dsn" content="${escaped}">\n<script src="traceway.js" defer></script>`

const file = '_site/index.html'
const html = await Bun.file(file).text()
if (!html.includes('<!--traceway-->')) {
  console.error('Placeholder <!--traceway--> not found in ' + file)
  process.exit(1)
}
await Bun.write(file, html.replace('<!--traceway-->', () => tags))
console.log('Traceway injected into ' + file)

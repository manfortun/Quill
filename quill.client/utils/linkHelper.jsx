export function getLink(links, ref) {
    return links.find(l => l.ref === ref);
}
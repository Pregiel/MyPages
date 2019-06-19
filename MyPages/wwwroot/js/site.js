
function mdToHtml(markdown) {
    var converter = new showdown.Converter,
        text = markdown.replace(/\&#xD;&#xA;/g, "\r\n"),
        html = converter.makeHtml(text);
    return html;
}
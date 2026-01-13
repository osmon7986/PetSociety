

function extractYoutubeId(url) {
    let regExp = /^.*(?:youtu.be\/|shorts\/|v\/|u\/\w\/|embed\/|watch\?v=|watch\?.+&v=)([^#&?]{11}).*/;
    var match = url.match(regExp);
    // 網址和 id 都不符合回傳null
    return (match && match[1].length === 11) ? match[1] : null;
}
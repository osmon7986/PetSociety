// 點選上傳圖片
$('#uploadBox').on('click', () => {
    $('#imageInput').click();
});

//拖曳到圖片框
$('#uploadBox').on('dragover', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $(this).addClass('dragover');
});
// 拖曳移開圖片框
$('#uploadBox').on('dragleave', (e) => {
    e.preventDefault();
    e.stopPropagation();
    $(this).removeClass('dragover');
});

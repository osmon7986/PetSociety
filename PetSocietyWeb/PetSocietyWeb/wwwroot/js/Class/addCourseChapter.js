// 新增章節按鈕
$('#addChapter').on('click', function () {
    let chapterIndex = $('.chapter-card').length + 1;
    let index = $('.chapter-card').length;

    let newChapter = `
                <div class="chapter-card card p-4 mb-4 shadow-sm">
                        <h5 class="mb-4 fw-bold">章節 ${chapterIndex}</h5>
                        <div class="mb-3">
                            <label class="form-label">章節標題</label>
                            <input type="text" name="Chapters[${index}].ChapterName" class="form-control" />
                            <input type="hidden" name="Chapters[${index}].SortOrder" data-val="true" data-val-required="章節標題為必填" />
                            <span data-valmsg-for="Chapters[${index}].ChapterName" class="text-danger" data-valmsg-replace="true"></span>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">章節大綱</label>
                            <textarea name="Chapters[${index}].Summary" class="form-control mt-2 summary" data-val="true"  rows="5"></textarea>
                            <span data-valmsg-for="Chapters[${index}].Summary" class="text-danger" data-valmsg-replace="true"></span>

                        </div>
                        <p class="sum-status text-secondary">
                                <span class="sumSpinner spinner-border spinner-border-sm me-2 d-none"></span>
                                <span class="sumText"></span>
                        </p>
                        <div class="mb-3">
                            <label class="form-label">章節影片</label>
                            <input type="text" name="Chapters[${index}].Video.VideoUrl" id="Chapters_${index}__Videos.VideoUrl" data-index="${index}" class="video-input form-control" />
                            <div class="ratio ratio-16x9 d-none" id="videoWrapper_${index}">
                                <iframe id="video_${index}" allowfullscreen></iframe>
                            </div>
                            <span data-valmsg-for="Chapters[${index}].Video.VideoUrl" class="text-danger" data-valmsg-replace="true"></span>
                        </div>
                 </div>`
    $('#chapters-container').append(newChapter);
    

});
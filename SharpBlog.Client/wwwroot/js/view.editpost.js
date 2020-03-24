'use strict';

tinymce.init({
    selector: '#post-content',
    plugins: ['autosave', 'preview', 'searchreplace', 'visualchars', 'image', 'link', 'media', 'fullscreen', 'code', 'codesample', 'table', 'hr', 'pagebreak', 'autoresize', 'nonbreaking', 'anchor', 'insertdatetime', 'advlist', 'lists', 'wordcount', 'imagetools'],
    menubar: "edit view format insert table",
    toolbar: 'formatselect | bold italic blockquote forecolor backcolor | imageupload link | alignleft aligncenter alignright  | numlist bullist outdent indent | fullscreen',
    autoresize_bottom_margin: 0,
    paste_data_images: true,
    image_advtab: true,
    file_picker_types: 'image',
    relative_urls: false,
    convert_urls: false,
    branding: true,
    init_instance_callback: function (editor) {
        editor.on("Dirty", function (e) {
            showTag("warning-dirty-post");
        });
    }
});

let isPublishedInitialValue = document.getElementById("IsPublished").checked;
let titleInitialValue = document.getElementById("Title").value;

document.getElementById("IsPublished").onclick = () => {
    markPostAsDirty(checkIfDirty());
};

document.getElementById("Title").oninput = () => {
    markPostAsDirty(checkIfDirty());
};

function showWarningPopUpOnPageLeave() {
    window.onbeforeunload = (e) => {
        if (document.activeElement.type === "submit") {
            return null;
        }
        return "You have unsaved changes are you sure you want to navigate away?";
    };
}

function hidePopUpOnPageLeave() {
    window.onbeforeunload = null;
}

function checkIfDirty() {
    return titleInitialValue !== document.getElementById("Title").value
        || isPublishedInitialValue !== document.getElementById("IsPublished").checked;
}

function markPostAsDirty(isDirty) {
    if (isDirty) {
        showTag('warning-dirty-post');
        showWarningPopUpOnPageLeave();
        return;
    }

    hideTag('warning-dirty-post');
    hidePopUpOnPageLeave();
}
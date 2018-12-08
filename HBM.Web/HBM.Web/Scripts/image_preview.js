var imageTypes = ['jpeg', 'jpg', 'png', 'bmp'];
function showImage(src, target) {
    var fr = new FileReader();
    fr.onload = function (e) {
        target.src = this.result;
    };
    fr.readAsDataURL(src.files[0]);
}
var uploadImage = function (obj) {
    var val = obj.value;
    var lastInd = val.lastIndexOf('.');
    var ext = val.slice(lastInd + 1, val.length);
    if (imageTypes.indexOf(ext) !== -1) {
        var id = $(obj).data('target');
        var src = obj;
        var target = $(id)[0];
        showImage(src, target);
    }
}
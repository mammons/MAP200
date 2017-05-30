$(document).ready(function () {
    $('#clearBtn').click(function () {
        let textBoxes = $('input:text');
        textBoxes.each(function (i, el) {
            el.attr('value', '');
        });
    });
    //console.log("Click event handler should be applied");
}

);

function clearTextBoxes() {
    alert("clearbutton clicked clearing");
    let textBoxes = $('input:text');
    for (var textBox in textBoxes) {
        textBox.innerHtml = "";
    }
}



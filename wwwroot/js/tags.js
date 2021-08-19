let index = 0;

function AddTag() {
    var tagEntry = document.getElementById("tagEntry");

    //searchResult stores the error text returned by the search function
    let searchResult = search(tagEntry.value);
    if (searchResult != null) {
        swalWithDarkButton.fire({
            html: `<span class='font-weight-bolder'>${searchResult.toUpperCase()}</span>`
        });
    }
    else {
        let newTag = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newTag;
    }
    tagEntry.value = '';
    return true;
}

function DeleteTag() {
    let tagCount = 1;

    let tagList = document.getElementById("TagList");
    if (!tagList) return false;

    if (tagList.selectIndex == -1) {
        swalWithDarkButton.fire({
            html: "<span class='cont-weight-bolder'>CHOOSE A TAG BEFORE DELETING</span>"
        });
        return true;
    }

    while (tagCount > 0) {
        if (tagList.selectedIndex >= 0) {
            tagList.options[tagList.selectedIndex] = null;
            --tagCount;
        }
        else {
            tagCount = 0;
            index--;
        }
    }
    return true;
}

//This is to select all entries when submitted to the HttpPost
$("form").on("submit", function () {
    $("#TagList option").prop("selected", "selected");
});

//This is to help with redirect from an error state in HttpGet
if (tagValues != '') {
    let tagArray = tagValues.split(",");
    for (let loop = 0; loop < tagArray.length; loop++) {
        ReplaceTag(tagArray[loop], loop);
        index++;
    }
}

function ReplaceTag(tag, index) {
    let newTag = new Option(tag, tag);
    document.getElementById("TagList").options[index] = newTag;
}

function search(str) {
    if (str == "") {
        return 'Empty tags are not permitted';
    }

    var tagsEl = document.getElementById("TagList");
    if (tagsEl) {
        let options = tagsEl.options;
        for (let index = 0; index < options.length; index++) {
            if (options[index].value == str)
                return `Duplicate tags are not permitted`;
        }
    }
}

//Swal Base
const swalWithDarkButton = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-danger btn-sm btn-block',
    },
    imageUrl: '/images/error.png',
    timer: 3000,
    buttonsStyling: false
});
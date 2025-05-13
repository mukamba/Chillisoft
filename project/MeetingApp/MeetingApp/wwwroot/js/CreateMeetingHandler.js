$(document).ready(function () {
    // When the meeting type dropdown changes
    $('#MeetingTypeDropdown').change(function () {
        var meetingTypeId = $(this).val();
        var minutesItemCheckboxGroup = $('#MinutesItemCheckboxGroup');

        // If a meeting type is selected
        if (meetingTypeId) {
            $.ajax({
                url: '@Url.Action("GetMinutesItemsByMeetingType", "Home")',
                data: { meetingTypeId: meetingTypeId },
                type: 'GET',
                success: function (data) {
                    minutesItemCheckboxGroup.empty();

                    // Check if data is returned
                    if (data && data.length > 0) {
                        data.forEach(function (item) {
                            // Create a checkbox for each minutes item
                            var checkbox = '<div class="form-check">' +
                                '<input class="form-check-input" type="checkbox" name="MinutesIDList" value="' + item.minutesItemID + '" id="MinutesItem_' + item.minutesItemID + '">' +
                                '<label class="form-check-label" for="MinutesItem_' + item.minutesItemID + '">' + item.title + ' - ' + item.description + '</label>' +
                                '</div>';
                            minutesItemCheckboxGroup.append(checkbox);
                        });
                    } else {
                        minutesItemCheckboxGroup.append('<div>No Minutes Items found</div>');
                    }
                },
                error: function () {
                    alert('An error occurred while retrieving the minutes items.');
                }
            });
        } else {
            // If no meeting type is selected, clear the checkbox group
            minutesItemCheckboxGroup.empty();
        }
    });

    // When the form is submitted
    $('form').submit(function (e) {
        var selectedItems = [];

        // Collect selected minutes items
        $('input[name="MinutesIDList"]:checked').each(function () {
            selectedItems.push($(this).val());
        });

        // Store the selected items in the hidden input
        $('#MinutesItemIds').val(selectedItems.join(','));

        // Optional: Validate if the user has selected at least one checkbox
        if (selectedItems.length === 0) {
            $('#MinutesItemError').text('Please select at least one minutes item.');
            e.preventDefault(); // Prevent form submission if no items are selected
        }
    });
});

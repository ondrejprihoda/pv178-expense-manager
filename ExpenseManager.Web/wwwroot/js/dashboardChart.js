document.addEventListener("DOMContentLoaded", function () {
    var ctx = document.getElementById('dashboardChart').getContext('2d');

    var categoryNames = JSON.parse(document.getElementById('categoryNames').value);
    var categoryAmounts = JSON.parse(document.getElementById('categoryAmounts').value);

    var chart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: categoryNames,
            datasets: [{
                label: 'Transaction Amounts by Category',
                data: categoryAmounts,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.5)',
                    'rgba(255, 205, 86, 0.5)',
                    'rgba(54, 162, 235, 0.5)',
                    'rgba(153, 102, 255, 0.5)',
                    'rgba(75, 192, 192, 0.5)',
                ],
                borderColor: [
                    'rgb(255, 99, 132)',
                    'rgb(255, 205, 86)',
                    'rgb(54, 162, 235)',
                    'rgb(153, 102, 255)',
                    'rgb(75, 192, 192)',
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                },
                tooltip: {
                    callbacks: {
                        label: function (tooltipItem) {
                            return tooltipItem.label + ': ' + tooltipItem.raw + ' CZK';
                        }
                    }
                }
            }
        }
    });
});
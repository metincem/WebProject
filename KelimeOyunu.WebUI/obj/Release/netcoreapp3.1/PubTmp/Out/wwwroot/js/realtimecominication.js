var connection = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.Critical).withUrl("/RTSorular").build();
$(function () {

    String.prototype.toHMS = function () {
        var sec_num = parseInt(this, 10);
        var hours = Math.floor(sec_num / 3600);
        var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
        var seconds = sec_num - (hours * 3600) - (minutes * 60);
        hours = (hours < 10) ? "0" + hours : hours;
        minutes = (minutes < 10) ? minutes = "0" + minutes : minutes;
        seconds = (seconds < 10) ? seconds = "0" + seconds : seconds;
        return (hours != "00") ? hours + ':' + minutes + ':' + seconds : minutes + ':' + seconds;
    }

    var letters = ["A", "B", "C", "Ç", "D", "E", "F", "G", "Ğ", "H", "I", "İ", "J", "K", "L", "M", "N", "O", "Ö", "P", "R", "S", "Ş", "T", "U", "Ü", "V", "Y", "Z"];

    function Letter(index, character) {
        var counter = 0;
        var inter = setInterval(function () {
            $("#box-" + index).html(letters[counter]);
            counter++;
            if (counter >= 29) {
                clearInterval(inter);
                $("#box-" + index).html(character);
            }
        }, 30);
    }
    var sure = 240;
    var sayac;
    var tahminsayac;
    var tahminSure;

    connection.start().then(function () {
        var idNo = $("#oturumID").val();
        connection.invoke("SurecBasladi", idNo);
    }).catch((err) => console.log(err))

    $("body").on("click", "#letter", function () {
        connection.invoke("HarfAlindi");
    });

    $("body").on("click", "#tahminle", function () {
        var sonuc = $("#wordGss").val().toString();
        connection.invoke("TahminEt", sonuc);
    });

    $("body").on("click", "#answer", function () {
        $(".crt").hide("fast");
        $(".gss").show("fast");
        $(".qs-time").show("fast");
        tahminSure = 19;
        tahminsayac = setInterval(function () {
            if (tahminSure == 0) {
                connection.invoke("Cevaplanmadi");
                clearInterval(tahminsayac);
                $(".qs-time").hide("fast");
            }
            else {
                $("#qstime").html(tahminSure.toString());
                tahminSure--;
            }
        }, 1000);
        clearInterval(sayac);
        connection.invoke("CevapVer");
        
        $("#wordGss").val("");
        $("#wordGss").focus();
    });

    $("body").on("click", "#siradaki", function () {
        connection.invoke("SiradakiSoru");
    });

    connection.on("tahminsonuc", (sonuc, puan, kelime, soruNo) => {
        if (sonuc == 0) {
            yanlisCevap();
        }
        else {
            soruArasi(puan, kelime);
            $("#ans-rs").css("color", "#27ae60");
            clearInterval(tahminsayac);
            if (soruNo == 14) {
                var zaman = 4;
                $("#sn-dv").html("5 saniye sonra sonuç sayfasına yönlendirileceksiniz.")
                setInterval(function () {
                    if (zaman == 0) {
                        window.location.href = "/Sonuc";
                    }
                    else {
                        $("#sn-dv").html(zaman + " saniye sonra sonuç sayfasına yönlendirileceksiniz.");
                        zaman--;
                    }
                }, 1000);
            }
            $("#wordGss").val("");
            $(".qs-time").hide("fast");
        }
    });

    connection.on("sorubildiri", (soru, soruNo, uzunluk, zaman, ttpuan) => {
        $("#soruP").html(soru);
        $("#soruNo").html("Soru " + soruNo);
        $("#ttpuan").html(ttpuan);
        $("#crrsc").html(uzunluk * 100);
        $(".qs-bt").hide("fast");
        $(".crt").show("fast");
        $(".gss").hide("fast");
        $(".qs-time").hide("fast");
        sure = zaman;
        for (i = 1; i <= 10; i++) {
            if (i <= uzunluk) {
                $("#box-" + i).show();
            }
            else {
                $("#box-" + i).hide();
            }
            $("#box-" + i).html("");
        }
        sayac = setInterval(function () {
            sure -= 1;
            if (sure == 0) {
                window.location.href = "/Sonuc";
            }
            var lclSure = sure.toString();
            $("#timeBox").html(lclSure.toHMS());
        }, 1000);
    });

    connection.on("harfbildiri", (harf, index, puan) => {
        $("#crrsc").html(puan);
        if (harf == "i")
            harf = "İ"
        else
            harf = harf.toUpperCase();
        Letter(index, harf.toUpperCase());
    });

    connection.on("cevapsiz", (puan, kelime, soruNo) => {
        soruArasi(puan, kelime);
        var puanA = $("#ttpuan").text();
        if (puan == puanA) {
            clearInterval(sayac);
        }
        if (soruNo == 14) {
            var zaman = 4;
            $("#sn-dv").html("5 saniye sonra sonuç sayfasına yönlendirileceksiniz.")
            setInterval(function () {
                if (zaman == 0) {
                    window.location.href = "/Sonuc";
                }
                else {
                    $("#sn-dv").html(zaman + " saniye sonra sonuç sayfasına yönlendirileceksiniz.");
                    zaman--;
                }
            }, 1000);
        }
        $("#ans-rs").css("color", "#e74c3c");

    });

    $("#wordGss").keypress(function (e) {
        if (e.which == 13 && $("#wordGss").css("display") != "none")
            $("#tahminle").trigger("click");
    });

    $("body").on("click", "#pass", function () {
        connection.invoke("Cevaplanmadi");
        clearInterval(tahminsayac);
        $(".qs-time").hide("fast");
    });

    function yanlisCevap() {
        $(".tryagain").show("fast");
        setTimeout(function () {
            $(".tryagain").hide("fast");
        }, 3000);
    }

    function soruArasi(puan, cevap) {
        $("#ans-rs").html(cevap.toUpperCase());
        $("#ttl-puan").html(puan);
        $(".qs-bt").show("fast");
    }
});
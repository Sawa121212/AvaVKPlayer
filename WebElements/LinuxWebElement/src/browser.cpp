#include "browser.hpp"

browser::browser(std::string _url, int _port) : QObject() {
    port = _port;
    url = _url;
    tcp = new tcp_helper(port);
    tcp->connect();
}

void browser::run_browser() {
    QWebEngineView *view = new QWebEngineView;
    connect(view, &QWebEngineView::urlChanged, this, &browser::url_changed);
    view->resize(800, 800);
    view->load(QUrl(url.c_str()));
    view->show();
}


void browser::url_changed(const QUrl &url) {
    qDebug() << url;

    QString url_string = url.url();
    tcp->write(url_string);
}

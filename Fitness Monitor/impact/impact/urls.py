from django.conf.urls import patterns, include, url

# Uncomment the next two lines to enable the admin:
from django.contrib import admin
admin.autodiscover()

urlpatterns = patterns('',
    # Examples:
    # url(r'^$', 'impact.views.home', name='home'),
    # url(r'^impact/', include('impact.foo.urls')),

    # Uncomment the admin/doc line below to enable admin documentation:
    # url(r'^admin/doc/', include('django.contrib.admindocs.urls')),

    # Uncomment the next line to enable the admin:
    url(r'^admin/', include(admin.site.urls)),
)

urlpatterns += patterns('fitnessmonitor.views',
    url(r'^test/$', "test", name="home"),
    url(r'^$', "home", name="home"),
    url(r'^login/$', "login_user", name="login_user"),
    url(r'^signout/$', "signout", name="signout"),
    url(r'^register/$', "register", name="register"),
    url(r'^new_session/$', "new_session", name="new_session"),
    url(r'^begin_round/$', "begin_round", name="begin_round"),
    url(r'^end_round/$', "end_round", name="end_round"),
)
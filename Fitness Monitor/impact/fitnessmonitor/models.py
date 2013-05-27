from django.db import models
from django.contrib.auth.models import User,UserManager
from sorl.thumbnail import ImageField
import datetime

# Create your models here.
class Practitioner(User):
    level = models.IntegerField(null=True, blank=True,default=0)
    image = ImageField(upload_to='profile_pictures', max_length=255, blank=True, default='')
    about = models.TextField(blank=True, max_length=512)
    session_no = models.IntegerField(null=True, blank=True,default=0)

    # def __unicode__(self):
    #     return self.first_name

class Session(models.Model):
    trainee = models.ForeignKey(Practitioner)
    start = models.DateTimeField(null=True)
    end = models.DateTimeField(null=True)
    rounds_num = models.IntegerField(null=True, blank=True,default=1)
    rounds_dur = models.IntegerField(null=True, blank=True,default=60)
    breaks_dur = models.IntegerField(null=True, blank=True,default=15)

class Round(models.Model):
    trainee = models.ForeignKey(Practitioner)
    session = models.ForeignKey(Session)
    start = models.DateTimeField(null=True)
    end = models.DateTimeField(null=True)
    number = models.IntegerField(null=True, blank=True,default=1)
    duration = models.IntegerField(null=True, blank=True,default=1)
from django.db import models
from django.contrib.auth.models import User,UserManager
import datetime

# Create your models here.
class Practitioner(User):
    level = models.IntegerField(null=True, blank=True,default=0)

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